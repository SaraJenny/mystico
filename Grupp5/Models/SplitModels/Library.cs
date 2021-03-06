﻿using Grupp5.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using MailKit.Net.Smtp;

namespace Grupp5.Models.SplitModels
{
    public static class Library
    {
        static public int GetTotalCostForEvent(Event myEvent)
        {
            decimal amount = 0;
            foreach (var expense in myEvent.Expense)
            {
                amount += expense.AmountInStandardCurrency;
            }

            return Convert.ToInt32(Math.Round(amount, MidpointRounding.AwayFromZero));
        }

        static public List<WhoOwesWho> WhoOweWho(Event myEvent)
        {
            Dictionary<int, decimal> userCredits = CreateDictionaryForUserCredits(myEvent);

            var creditors = new List<User>();
            var debitors = new List<User>();

            foreach (var person in myEvent.ParticipantsInEvent)
            {

                if (userCredits[person.UserId] < 0)
                {
                    userCredits[person.UserId] = Math.Abs(userCredits[person.UserId]);
                    debitors.Add(person.User);
                }
                else if (userCredits[person.UserId] > 0)
                    creditors.Add(person.User);
            }

            var transaction = new List<WhoOwesWho>();

            creditors = creditors.OrderByDescending(o => userCredits[o.Id]).ToList();
            debitors = debitors.OrderByDescending(d => userCredits[d.Id]).ToList();

            foreach (var debitor in debitors)
            {
                do
                {
                    if (userCredits[debitor.Id] > userCredits[creditors[0].Id])
                    {
                        var transferSum = Math.Round(userCredits[creditors[0].Id], MidpointRounding.AwayFromZero);

                        transaction.Add(new WhoOwesWho { Debitor = debitor, Creditor = creditors[0], Amount = userCredits[creditors[0].Id] });
                        userCredits[debitor.Id] -= userCredits[creditors[0].Id];
                        creditors.Remove(creditors[0]);
                    }
                    else if (userCredits[debitor.Id] < userCredits[creditors[0].Id])
                    {
                        var transferSum = Math.Round(userCredits[debitor.Id], MidpointRounding.AwayFromZero);

                        transaction.Add(new WhoOwesWho { Debitor = debitor, Creditor = creditors[0], Amount = userCredits[debitor.Id] });
                        userCredits[creditors[0].Id] -= userCredits[debitor.Id];
                        userCredits[debitor.Id] = 0;
                    }
                    else
                    {
                        var transferSum = Math.Round(userCredits[debitor.Id], MidpointRounding.AwayFromZero);

                        transaction.Add(new WhoOwesWho { Debitor = debitor, Creditor = creditors[0], Amount = userCredits[debitor.Id] });
                        userCredits[debitor.Id] = 0;
                        creditors.Remove(creditors[0]);
                    }

                } while (Math.Round(userCredits[debitor.Id]) > 0);
            }

            return transaction;

        }

        internal static List<SplitDetailsVM> ConvertExpenseToSplitDetailsVM(List<Expense> expenses, List<PayersForExpense> objections)
        {
            var expensesVM = new List<SplitDetailsVM>();

            foreach (var exp in expenses)
            {
                SplitDetailsVM expense = new SplitDetailsVM();

                expense.ExpenseId = exp.Id;

                expense.ExpenseInfo = new WhoBoughtWhatVM
                {
                    PurchaserId = exp.PurchaserId,
                    PurchaserEmail = exp.Purchaser.Email,
                    PurchaserFirstName = exp.Purchaser.FirstName,
                    PurchaserLastName = exp.Purchaser.LastName,
                    Amount = Convert.ToInt32(exp.Amount),
                    CurrencyCode = exp.Currency.CurrencyCode,
                    ExpenseDescription = exp.Description,
                    Date = exp.Date.ToString("u").Replace(" 00:00:00Z", "")

                };

                expense.ExpenseInfo.payers = new List<PayerVM>();

                foreach (var payer in exp.PayersForExpense)
                {
                    expense.ExpenseInfo.payers.Add(new PayerVM
                    {
                        Id = payer.UserId,
                        FirstName = payer.User.FirstName,
                        LastName = payer.User.LastName,
                        Email = payer.User.Email
                    });
                }

                var objectionsForExpense = objections.Where(o => o.ExpenseId == exp.Id).ToList();

                if (objectionsForExpense.Count() > 0)
                {
                    expense.ObjectionExists = true;
                    expense.ObjectionMessage = new List<string>();

                    foreach (var objection in objectionsForExpense)
                    {
                        expense.ObjectionMessage.Add(objection.ObjectionDescription);
                    }
                }

                expensesVM.Add(expense);
            }

            return expensesVM;

        }

        internal static SelectListItem[] ConvertParticipantsToSelectListItem(Event myEvent, int userId)
        {
            var eventItems = new List<SelectListItem>();
            foreach (var participant in myEvent.ParticipantsInEvent)
            {
                if (participant.UserId == userId)
                    eventItems.Add(new SelectListItem { Text = $"{participant.User.FirstName} {participant.User.LastName}", Value = participant.UserId.ToString(), Selected = true });
                else
                    eventItems.Add(new SelectListItem { Text = $"{participant.User.FirstName} {participant.User.LastName}", Value = participant.UserId.ToString() });
            }

            return eventItems.ToArray();
        }

        internal static List<PayerVM> ConvertToListOfPayersVM(List<User> myParticipants, List<User> myPayers)
        {
            var participantsWithMarkedPayer = new List<PayerVM>();

            foreach (var participant in myParticipants)
            {
                var payerVM = new PayerVM
                {
                    Id = participant.Id,
                    FirstName = participant.FirstName,
                    LastName = participant.LastName,
                    Email = participant.Email
                };

                if (myPayers.Contains(participant))
                {
                    payerVM.IsPayer = true;
                }

                participantsWithMarkedPayer.Add(payerVM);
            }

            return participantsWithMarkedPayer;
        }

        internal static List<UserVM> ConvertUsersToUsersVM(List<User> users)
        {
            var userVMlist = new List<UserVM>();

            foreach (var person in users)
            {
                userVMlist.Add(new UserVM
                {
                    Id = person.Id,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Email = person.Email
                });
            }

            return userVMlist;
        }

        private static Dictionary<int, decimal> CreateDictionaryForUserCredits(Event myEvent)
        {

            var userCredits = new Dictionary<int, Decimal>();

            foreach (var participant in myEvent.ParticipantsInEvent)
            {
                userCredits.Add(participant.UserId, 0);
            }

            foreach (var expense in myEvent.Expense)
            {
                var amount = expense.AmountInStandardCurrency;
                var purchaser = expense.PurchaserId;
                var payers = expense.PayersForExpense;

                var debt = (amount / payers.Count);

                if (payers.Where(p => p.UserId == purchaser).Count() > 0)
                {
                    foreach (var payer in payers)
                    {
                        if (payer.UserId == purchaser)
                        {
                            userCredits[payer.UserId] += (amount - debt);
                        }
                        else
                        {
                            //var userId = myEvent.ParticipantsInEvent.Where(p => p.UserId == payer.UserId).First().UserId;
                            userCredits[payer.UserId] -= debt;
                        }
                    }
                }
                else
                {
                    userCredits[purchaser] += amount;
                    foreach (var payer in payers)
                    {
                        userCredits[payer.UserId] -= debt;
                    }
                }
            }

            return userCredits;
        }

        internal static SelectListItem[] ConvertParticipantsToSelectListItemEditVersion(Event myEvent, Expense myExpense)
        {
            var eventItems = new List<SelectListItem>();
            foreach (var participant in myEvent.ParticipantsInEvent)
            {
                if (participant.UserId == myExpense.PurchaserId)
                    eventItems.Add(new SelectListItem { Text = $"{participant.User.FirstName} {participant.User.LastName}", Value = participant.UserId.ToString(), Selected = true });
                else
                    eventItems.Add(new SelectListItem { Text = $"{participant.User.FirstName} {participant.User.LastName}", Value = participant.UserId.ToString() });
            }

            return eventItems.ToArray();
        }

        private static Dictionary<int, decimal> CreateDictionaryForUserTotals(Event myEvent)
        {
            var userTotals = new Dictionary<int, Decimal>();

            foreach (var participant in myEvent.ParticipantsInEvent)
            {
                userTotals.Add(participant.UserId, 0);
            }

            foreach (var expense in myEvent.Expense)
            {
                var amount = expense.AmountInStandardCurrency;
                var payers = expense.PayersForExpense;

                foreach (var payer in payers)
                {
                    var debt = (amount / payers.Count);
                    var userId = myEvent.ParticipantsInEvent.Where(p => p.UserId == payer.UserId).First().UserId;
                    userTotals[userId] += debt;
                }
            }

            return userTotals;
        }

        static public int GetUserTotalById(Event myEvent, int userId)
        {
            var dictionary = CreateDictionaryForUserTotals(myEvent);
            return Convert.ToInt32(Math.Round(dictionary[userId], MidpointRounding.AwayFromZero));
        }

        static public int GetUserStatusById(Event myEvent, int userId)
        {
            var dictionary = CreateDictionaryForUserCredits(myEvent);
            return Convert.ToInt32(Math.Round(dictionary[userId], MidpointRounding.AwayFromZero));
        }

        static public SelectListItem[] ConvertCurrencyToSelectListItem(List<Currency> allCurrencies)
        {
            var theList = new List<SelectListItem>();

            foreach (var item in allCurrencies)
            {
                theList.Add(new SelectListItem() { Text = item.CurrencyCode, Value = item.Id.ToString() });
            }

            return theList.ToArray();
        }

        static public ListOfEventsVM[] ConvertToListOfEventsVMArray(List<Event> events)
        {
            var myList = new List<ListOfEventsVM>();

            foreach (var item in events)
            {
                myList.Add(new ListOfEventsVM() { Id = item.Id, EventName = item.EventName });
            }

            return myList.ToArray();
        }

        internal static SplitUpdateEventVM ConvertEventToSplitEventVM(Event myEvent)
        {
            var splitEvent = new SplitUpdateEventVM
            {
                Name = myEvent.EventName,
                Description = myEvent.Description,
                AlreadyParticipants = new List<UserVM>(),
                EventId = myEvent.Id,
                SelectedCurrency = myEvent.StandardCurrencyId.ToString(),
                ExpenseCurrencyId = myEvent.ExpenseCurrencyId.ToString()
            };

            foreach (var participant in myEvent.ParticipantsInEvent)
            {
                splitEvent.AlreadyParticipants.Add(new UserVM
                {
                    FirstName = participant.User.FirstName,
                    LastName = participant.User.LastName,
                    Email = participant.User.Email,
                    Id = participant.UserId
                });

            }

            return splitEvent;
        }

        internal static SelectListItem[] ConvertEventToSelectListItem(List<Event> myEvents)
        {
            var eventItems = new List<SelectListItem>();
            foreach (var xEvent in myEvents)
            {
                eventItems.Add(new SelectListItem { Text = xEvent.EventName, Value = xEvent.Id.ToString() });
            }

            return eventItems.ToArray();
        }

        internal static List<WhoOwesWhoVM> ConvertWhoOwesWho(List<WhoOwesWho> transactions)
        {
            List<WhoOwesWhoVM> transactionsVM = new List<WhoOwesWhoVM>();

            foreach (var item in transactions)
            {
                transactionsVM.Add(new WhoOwesWhoVM
                {
                    Amount = Convert.ToInt32(Math.Round(item.Amount, MidpointRounding.AwayFromZero)),
                    DebitorId = item.Debitor.Id,
                    DebitorFirstName = item.Debitor.FirstName,
                    DebitorLastName = item.Debitor.LastName,
                    DebitorEmail = item.Debitor.Email,
                    CreditorId = item.Creditor.Id,
                    CreditorFirstName = item.Creditor.FirstName,
                    CreditorLastName = item.Creditor.LastName,
                    CreditorEmail = item.Creditor.Email
                });
            }

            return transactionsVM;
        }

        internal static SplitExpenseVM ConvertToSplitExpenseVM(Expense myExpense)
        {
            return new SplitExpenseVM
            {
                Amount = Math.Round(myExpense.Amount, 2).ToString(System.Globalization.CultureInfo.InvariantCulture),
                Date = myExpense.Date.ToString("u").Replace(" 00:00:00Z", ""),
                Description = myExpense.Description,
                SelectedCurrency = myExpense.CurrencyId,
                SelectedEvent = myExpense.EventId,

            };
        }


    }
}

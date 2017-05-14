using Grupp5.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grupp5.Models.SplitModels
{
    public static class Library
    {
    static public int GetTotalCostForEvent(Event myEvent)
        {
            decimal amount = 0;
            foreach (var expense in myEvent.Expense)
            {
                amount += expense.Amount;
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
            foreach (var item in expenses)
            {
                var payerNames = "";
                foreach (var payer in item.PayersForExpense)
                {
                    payerNames += $"{payer.User.FirstName}, ";
                }
                payerNames.TrimEnd(' ');
                payerNames.TrimEnd(',');

                var objection = objections.Where(o => o.ExpenseId == item.Id).FirstOrDefault();
                SplitDetailsVM expense = new SplitDetailsVM();
                expense.expenseInformation = $"" +
                    $"{item.Purchaser.FirstName} har lagt ut " +
                    $"{Math.Round(item.Amount, 2, MidpointRounding.AwayFromZero)} {item.Currency.CurrencyCode} " +
                    $"för {item.Description} som " +
                    $"{payerNames} ska betala för";
                if (objection != null)
                {
                    expense.objectionExists = objection.Objection;
                    expense.objectionMessage = objection.ObjectionDescription;
                }
                expensesVM.Add(expense);

        }
            return expensesVM;
            
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
            var amount = expense.Amount;
            var purchaser = expense.PurchaserId;
            var payers = expense.PayersForExpense;

            foreach (var payer in payers)
            {
                var debt = (amount / payers.Count);
                if (payer.UserId == purchaser)
                {
                    var userId = myEvent.ParticipantsInEvent.Where(p => p.UserId == payer.UserId).First().UserId;
                    userCredits[userId] += (amount - debt);
                }
                else
                {
                    var userId = myEvent.ParticipantsInEvent.Where(p => p.UserId == payer.UserId).First().UserId;
                    userCredits[userId] -= debt;
                }
            }
        }

        return userCredits;
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
            var amount = expense.Amount;
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

    static public SplitIndexVM[] ConvertToSplitIndexVMArray(List<Event> events)
    {
        var myList = new List<SplitIndexVM>();

        foreach (var item in events)
        {
            myList.Add(new SplitIndexVM() { Id = item.Id, EventName = item.EventName });
        }

        return myList.ToArray();
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
                    Amount = Convert.ToInt32(item.Amount),
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
}
}

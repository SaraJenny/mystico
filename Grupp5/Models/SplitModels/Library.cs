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

        static public List<string> WhoOweWho(Event myEvent)
        {
            Dictionary<int, decimal> userCredits = CreateDictionaryForUserCredits(myEvent);

            var creditors = new List<ParticipantsInEvent>();
            var debitors = new List<ParticipantsInEvent>();

            foreach (var person in myEvent.ParticipantsInEvent)
            {
                if (userCredits[person.UserId] < 0)
                {
                    userCredits[person.UserId] = Math.Abs(userCredits[person.UserId]);
                    debitors.Add(person);
                }
                else if (userCredits[person.UserId] > 0)
                    creditors.Add(person);
            }

            var transaction = new List<string>();

            creditors = creditors.OrderByDescending(o => userCredits[o.UserId]).ToList();
            debitors = debitors.OrderByDescending(d => userCredits[d.UserId]).ToList();

            foreach (var debitor in debitors)
            {
                do
                {
                    if (userCredits[debitor.UserId] > userCredits[creditors[0].UserId])
                    {
                        var transferSum = Math.Round(userCredits[creditors[0].UserId], MidpointRounding.AwayFromZero);

                        transaction.Add($"{debitor.User.FirstName} ska överföra {transferSum} kr till {creditors[0].User.FirstName}");
                        userCredits[debitor.UserId] -= userCredits[creditors[0].UserId];
                        creditors.Remove(creditors[0]);
                    }
                    else if (userCredits[debitor.UserId] < userCredits[creditors[0].UserId])
                    {
                        var transferSum = Math.Round(userCredits[debitor.UserId], MidpointRounding.AwayFromZero);

                        transaction.Add($"{debitor.User.FirstName} ska överföra {transferSum} kr till {creditors[0].User.FirstName}");
                        userCredits[creditors[0].UserId] -= userCredits[debitor.UserId];
                        userCredits[debitor.UserId] = 0;
                    }
                    else
                    {
                        var transferSum = Math.Round(userCredits[debitor.UserId], MidpointRounding.AwayFromZero);

                        transaction.Add($"{debitor.User.FirstName} ska överföra {transferSum} kr till {creditors[0].User.FirstName}");
                        userCredits[debitor.UserId] = 0;
                        creditors.Remove(creditors[0]);
                    }

                } while (Math.Round(userCredits[debitor.UserId]) > 0);
            }

            return transaction;

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
            return Convert.ToInt32(Math.Round(dictionary[userId],MidpointRounding.AwayFromZero));
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
    }
}

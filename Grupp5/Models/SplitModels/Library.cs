using Grupp5.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupp5.Models.SplitModels
{
    public static class Library
    {
        static public List<string> WhoOweWho(Event myEvent)
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
                        //debitors.Remove(debitor);
                    }
                    else
                    {
                        var transferSum = Math.Round(userCredits[debitor.UserId], MidpointRounding.AwayFromZero);

                        transaction.Add($"{debitor.User.FirstName} ska överföra {transferSum} kr till {creditors[0].User.FirstName}");
                        userCredits[debitor.UserId] = 0;
                        creditors.Remove(creditors[0]);
                        //debitors.Remove(debitor);
                    }

                } while (Math.Round(userCredits[debitor.UserId]) > 0);
            }

            return transaction;

        }
    }
}

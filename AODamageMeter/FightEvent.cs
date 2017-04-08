﻿using AODamageMeter.FightEvents;
using AODamageMeter.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AODamageMeter
{
    public abstract class FightEvent
    {
        protected readonly DamageMeter _damageMeter;
        protected readonly Fight _fight;

        protected FightEvent(DamageMeter damageMeter, Fight fight, DateTime timestamp, string description)
        {
            _damageMeter = damageMeter;
            _fight = fight;
            Timestamp = timestamp;
            Description = description;
        }

        public static async Task<FightEvent> Create(DamageMeter damageMeter, Fight fight, string line)
        {
            int lastIndexOfArrayPart = line.IndexOf(']');
            string[] arrayPart = line.Substring(1, lastIndexOfArrayPart - 1).Split(',')
                .Select(p => p.Trim('"'))
                .ToArray();
            string eventName = arrayPart[1];
            DateTime timestamp = DateTimeHelper.DateTimeLocalFromUnixSeconds(long.Parse(arrayPart[3]));
            string description = line.Substring(lastIndexOfArrayPart + 1);

            if (eventName == OtherHitByOther.EventName) return await OtherHitByOther.Create(damageMeter, fight, timestamp, description);
            if (eventName == OtherHitByNano.EventName) return await OtherHitByNano.Create(damageMeter, fight, timestamp, description);
            if (eventName == YouHitOtherWithNano.EventName) return await YouHitOtherWithNano.Create(damageMeter, fight, timestamp, description);
            if (eventName == MeGotHealth.EventName) return await MeGotHealth.Create(damageMeter, fight, timestamp, description);
            throw new NotSupportedException($"{eventName}: {description}");
        }

        public abstract string Key { get; }
        public abstract string Name { get; }
        public DateTime Timestamp { get; }
        public string Description { get; }
        public FightCharacter Source { get; protected set; }
        public FightCharacter Target { get; protected set; }
        public ActionType ActionType { get; protected set; }
        public int Amount { get; protected set; }
        public DamageType? DamageType { get; protected set; }
        public Modifier? Modifier { get; protected set; }

        protected bool TryMatch(Regex regex, out Match match)
            => (match = regex.Match(Description)).Success;

        protected bool TryMatch(Regex regex, out Match match, out bool success)
        {
            match = regex.Match(Description);
            success = match.Success;
            return success;
        }

        protected async Task SetSourceAndTarget(Match match, int sourceIndex, int targetIndex)
        {
            var fightCharacters = await _fight.GetOrCreateFightCharacters(match.Groups[sourceIndex].Value, match.Groups[targetIndex].Value);
            Source = fightCharacters[0];
            Target = fightCharacters[1];
        }

        protected async Task SetSource(Match match, int index)
            => Source = await _fight.GetOrCreateFightCharacter(match.Groups[index].Value);

        protected async Task SetTarget(Match match, int index)
            => Target = await _fight.GetOrCreateFightCharacter(match.Groups[index].Value);

        protected void SetSourceAndTargetAsOwner()
            => Source = Target = _fight.GetOrCreateFightCharacter(_damageMeter.Owner);

        protected void SetSourceAsOwner()
            => Source = _fight.GetOrCreateFightCharacter(_damageMeter.Owner);

        protected void SetTargetAsOwner()
            => Target = _fight.GetOrCreateFightCharacter(_damageMeter.Owner);

        protected void SetAmount(Match match, int index)
            => Amount = int.Parse(match.Groups[index].Value);

        protected void SetDamageType(Match match, int index)
            => DamageType = DamageTypeHelpers.GetDamageType(match.Groups[index].Value);











        private void SetAttributes()
        {
            int indexOfSource, lengthOfSource,
                indexOfTarget, lengthOfTarget,
                indexOfAmount, lengthOfAmount,
                indexOfAmountType, lengthOfAmountType;

            switch (Key)
            {
                //You hit TARGET for AMOUNT points of AMOUNTTYPE damage.
                //Your damage shield hit TARGET for AMOUNT points of damage.
                case "08":

                    if (Line.StartsWith("Your"))
                    {
                        indexOfTarget = 23;
                        lengthOfTarget = Line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = Line.IndexOf(" points ") - indexOfAmount;

                        ActionType = "Damage";
                        Source = owningCharacterName;
                        Target = Line.Substring(indexOfTarget, lengthOfTarget);
                        Amount = Convert.ToInt32(Line.Substring(indexOfAmount, lengthOfAmount));
                        DamageType = "Shield";
                    }
                    else
                    {
                        indexOfTarget = 8;
                        lengthOfTarget = Line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = Line.IndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                        lengthOfAmountType = Line.Length - 8 - indexOfAmountType;

                        ActionType = "Damage";
                        Source = owningCharacterName;
                        Target = Line.Substring(indexOfTarget, lengthOfTarget);
                        Amount = Convert.ToInt32(Line.Substring(indexOfAmount, lengthOfAmount));
                        DamageType = Line.Substring(indexOfAmountType, lengthOfAmountType);

                        //You hit TARGET for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                        if (Line[Line.Length - 1] == '!')
                        {
                            Modifier = "Crit";
                        }
                        //You hit TARGET for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                        else if (Line[Line.Length - 2] == 't')
                        {
                            Modifier = "Glance";
                        }
                    }

                    break;

                //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage.
                //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                //Someone's reflect shield hit you for AMOUNT points of damage.
                //Someone's damage shield hit you for AMOUNT points of damage.
                //You absorbed AMOUNT points of AMOUNTTYPE damage.
                case "06":

                    //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage.
                    if (!Line.StartsWith("You absorbed ") && !Line.StartsWith("Someone's"))
                    {

                        indexOfSource = 0;
                        lengthOfSource = Line.IndexOf(" hit ") - indexOfSource;

                        indexOfAmount = indexOfSource + lengthOfSource + 13;
                        lengthOfAmount = Line.LastIndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                        lengthOfAmountType = Line.Length - 8 - indexOfAmountType;

                        ActionType = "Damage";
                        Source = Line.Substring(indexOfSource, lengthOfSource);

                        //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                        if (Line[Line.Length - 1] == '!')
                        {
                            lengthOfAmountType = Line.Length - 22 - indexOfAmountType;
                            Modifier = "Crit";
                        }
                        //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                        else if (Line[Line.Length - 2] == 't')
                        {
                            lengthOfAmountType = Line.Length - 22 - indexOfAmountType;
                            Modifier = "Glance";
                        }
                        DamageType = Line.Substring(indexOfAmountType, lengthOfAmountType);
                    }
                    //You absorbed AMOUNT points of AMOUNTTYPE damage.
                    else if (!Line.StartsWith("Someone's"))
                    {

                        indexOfAmount = 13;
                        lengthOfAmount = Line.IndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + 8;
                        lengthOfAmountType = Line.Length - 8 - indexOfAmountType;

                        ActionType = "Absorb";
                        Source = owningCharacterName;
                        DamageType = Line.Substring(indexOfAmountType, lengthOfAmountType);
                    }
                    //Someone's reflect shield hit you for AMOUNT points of damage.
                    //Someone's damage shield hit you for AMOUNT points of damage.
                    else
                    {
                        //Someone's damage shield hit you for AMOUNT points of damage.
                        if (Line[10] == 'd')
                        {
                            indexOfAmount = 36;
                            lengthOfAmount = Line.IndexOf(" points ") - indexOfAmount;
                            DamageType = "Shield";
                        }
                        //Someone's reflect shield hit you for AMOUNT points of damage.
                        else
                        {
                            indexOfAmount = 37;
                            lengthOfAmount = Line.IndexOf(" points ") - indexOfAmount;
                            DamageType = "Reflect";
                        }
                    }

                    Target = owningCharacterName;
                    Amount = Convert.ToInt32(Line.Substring(indexOfAmount, lengthOfAmount));

                    break;

                //SOURCE tried to hit you, but missed!
                //SOURCE tries to attack you with Brawl, but misses!
                //SOURCE tries to attack you with FastAttack, but misses!
                //SOURCE tries to attack you with FlingShot, but misses!
                case "13":

                    indexOfSource = 0;

                    if (Line.LastIndexOf("you,") == -1)
                    {

                        lengthOfSource = Line.IndexOf(" tries ") - indexOfSource;
                        //should be changed in future
                        DamageType = "SpecialAttack";

                    }
                    else
                    {
                        lengthOfSource = Line.IndexOf(" tried ") - indexOfSource;
                        DamageType = "Auto Attack";
                    }

                    ActionType = "Damage";
                    Source = Line.Substring(indexOfSource, lengthOfSource);
                    Target = owningCharacterName;

                    break;

                //You got nano from SOURCE for AMOUNT points.
                case "16":

                    indexOfSource = 18;
                    lengthOfSource = Line.IndexOf(" for ") - indexOfSource;

                    indexOfAmount = indexOfSource + lengthOfSource + 5;
                    lengthOfAmount = Line.Length - 8 - indexOfAmount;

                    ActionType = "Heal";
                    Source = Line.Substring(indexOfSource, lengthOfSource);
                    Target = owningCharacterName;
                    Amount = Convert.ToInt32(Line.Substring(indexOfAmount, lengthOfAmount));
                    DamageType = "Nano";

                    break;

                //You tried to hit TARGET, but missed!
                case "12":


                    indexOfTarget = 17;
                    lengthOfTarget = Line.Length - 13 - indexOfTarget;

                    ActionType = "Miss";
                    Source = owningCharacterName;
                    Target = Line.Substring(indexOfTarget, lengthOfTarget);

                    break;

                //You increased nano on TARGET for AMOUNT points.
                case "17":

                    indexOfTarget = 22;
                    lengthOfTarget = Line.LastIndexOf(" for ") - indexOfTarget;

                    indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                    lengthOfAmount = Line.Length - 8 - indexOfAmount;

                    ActionType = "Heal";
                    Source = owningCharacterName;
                    Target = Line.Substring(indexOfTarget, lengthOfTarget);
                    Amount = Convert.ToInt32(Line.Substring(indexOfAmount, lengthOfAmount));
                    DamageType = "Nano";

                    break;

                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                case "09":

                    indexOfSource = 0;
                    lengthOfSource = Line.IndexOf(" hit ") - indexOfSource;
                    // + 5 to skip the " hit " indices
                    indexOfTarget = lengthOfSource + 5;
                    lengthOfTarget = Line.IndexOf(" for ") - indexOfTarget;
                    // + 5 to skip the " for " indices
                    indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                    lengthOfAmount = Line.LastIndexOf(" points ") - indexOfAmount;
                    // + 8 to skip the " points " indices, + 3 to skip the "of " indices
                    indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                    lengthOfAmountType = Line.Length - 8 - indexOfAmountType;

                    //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                    if (Line[Line.Length - 1] == '!')
                    {
                        lengthOfAmountType = Line.Length - 22 - indexOfAmountType;
                        Modifier = "Crit";
                    }
                    //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                    else if (Line[Line.Length - 2] == 't')
                    {
                        lengthOfAmountType = Line.Length - 22 - indexOfAmountType;
                        Modifier = "Glance";
                    }
                    
                    ActionType = "Damage"; //PetDamage
                    Source = Line.Substring(indexOfSource, lengthOfSource);
                    Target = Line.Substring(indexOfTarget, lengthOfTarget);
                    Amount = Convert.ToInt32(Line.Substring(indexOfAmount, lengthOfAmount));
                    DamageType = Line.Substring(indexOfAmountType, lengthOfAmountType);
                    Source = Line.Substring(indexOfSource, lengthOfSource);

                    break;


                //You lost 1548140 xp.
                case "0b":
                    /*
                    indexOfTimeStart = 37;
                    
                    */
                    break;

                //Executing Nano Program: Composite Attribute Boost.
                //Wait for current nano program execution to finish.
                //Unable to execute nano program. You can't execute this nano on the target.
                case "18":

                    ActionType = "Utility";
                    /*
                    indexOfTimeStart = 40;
                    indexOfMessageStart = indexOfTimeStart + timeLength + 1;

                    eventTime = Convert.ToInt32(line.Substring(indexOfTimeStart, timeLength));
                    */
                    break;
            }
        }
    }
}

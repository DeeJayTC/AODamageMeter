﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anarchy_Online_Damage_Meter
{
    class Event
    {
        private string key = "";
        private int timeStamp = 0;
        private string action = "";
        private string source = "";
        private string target = "";
        private int amount = 0;
        private string amountType = "";
        private string modifier = "";


        public Event(string line)
        {
            SetKey(line);
            SetTime(key, line);
            SetAttributes(key, ParseMessage(key, line));
        }

        public string GetKey()
        {
            return key;
        }

        public int GetTimeStamp()
        {
            return timeStamp;
        }

        public int GetAmount()
        {
            return amount;
        }

        public string GetSource()
        {
            return source;
        }

        public string GetTarget()
        {
            return target;
        }

        public string GetAction()
        {
            return action;
        }

        public string GetAmountType()
        {
            return amountType;
        }

        public string GetModifier()
        {
            return modifier;
        }

        private void SetKey(string line)
        {
            //EXAMPLE LINE:
            //["#000000004200000a#","Other hit by other","",1457816347]SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage.
            //Digits indexed at 17 and 18, "0a" in this example. 
            key = line.Substring(17, 2);
        }

        private void SetTime(string key, string line)
        {
            int timeLength = 10;
            switch (key)
            {
                case "0a":
                    timeStamp = Convert.ToInt32(line.Substring(46, timeLength));
                    break;
                case "04":
                    timeStamp = Convert.ToInt32(line.Substring(45, timeLength));
                    break;
                case "15":
                    timeStamp = Convert.ToInt32(line.Substring(41, timeLength));
                    break;
                case "08":
                    timeStamp = Convert.ToInt32(line.Substring(41, timeLength));
                    break;
                case "06":
                    timeStamp = Convert.ToInt32(line.Substring(45, timeLength));
                    break;
                case "13":
                    timeStamp = Convert.ToInt32(line.Substring(40, timeLength));
                    break;
                case "16":
                    timeStamp = Convert.ToInt32(line.Substring(39, timeLength));
                    break;
                case "12":
                    timeStamp = Convert.ToInt32(line.Substring(39, timeLength));
                    break;
                case "17":
                    timeStamp = Convert.ToInt32(line.Substring(41, timeLength));
                    break;
                case "09":
                    timeStamp = Convert.ToInt32(line.Substring(49, timeLength));
                    break;
                case "0b":
                    timeStamp = Convert.ToInt32(line.Substring(37, timeLength));
                    break;
                case "18":
                    timeStamp = Convert.ToInt32(line.Substring(40, timeLength));
                    break;
            }
        }

        private string ParseMessage(string key, string line)
        {
            switch (key)
            {
                case "0a":
                    return line.Substring(57, line.Length - 57);
                case "04":
                    return line.Substring(56, line.Length - 56);
                case "15":
                    return line.Substring(52, line.Length - 52);
                case "08":
                    return line.Substring(52, line.Length - 52);
                case "06":
                    return line.Substring(56, line.Length - 56);
                case "13":
                    return line.Substring(51, line.Length - 51);
                case "16":
                    return line.Substring(50, line.Length - 50);
                case "12":
                    return line.Substring(50, line.Length - 50);
                case "17":
                    return line.Substring(52, line.Length - 52);
                case "09":
                    return line.Substring(60, line.Length - 60);
                case "0b":
                    return line.Substring(48, line.Length - 48);
                case "18":
                    return line.Substring(51, line.Length - 51);
            }
            return "";
        }

        private void SetAttributes(string key, string line)
        {
            int indexOfSource, lengthOfSource;
            int indexOfTarget, lengthOfTarget;
            int indexOfAmount, lengthOfAmount;
            int indexOfAmountType, lengthOfAmountType;

            switch (key)
            {
                //Every "you", "me" == SOURCE
                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage.
                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                //SOURCE's reflect shield hit TARGET for AMOUNT points of damage.
                //SOURCE's damage shield hit TARGET for AMOUNT points of damage.
                //Something hit TARGET for AMOUNT points of damage by damage shield.
                //Something hit TARGET for AMOUNT points of damage by reflect shield.
                case "0a":

                    if (line.LastIndexOf("Someone absorbed ") != -1)
                    {
                        break;
                    }
                    //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage.
                    if (line.IndexOf(" shield hit ") == -1 && line.IndexOf("shield.") == -1)
                    {
                        indexOfSource = 0;
                        lengthOfSource = line.IndexOf(" hit ") - indexOfSource;
                        // + 5 to skip the " hit " indices
                        indexOfTarget = indexOfSource + lengthOfSource + 5;
                        lengthOfTarget = line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                        lengthOfAmountType = line.IndexOf(" damage.") - indexOfAmountType;

                        amountType = line.Substring(indexOfAmountType, lengthOfAmountType);
                        source = line.Substring(indexOfSource, lengthOfSource);

                        //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                        if (line[line.Length - 1] == '!')
                        {
                            modifier = "Crit";
                        }
                        //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                        else if (line[line.Length - 2] == 't')
                        {
                            modifier = "Glance";
                        }
                    }
                    //SOURCE's reflect shield hit TARGET for AMOUNT points of damage.
                    //SOURCE's damage shield hit TARGET for AMOUNT points of damage.
                    else if (line.IndexOf(" shield hit ") != -1)
                    {
                        indexOfSource = 0;
                        //SOURCE's reflect shield hit TARGET for AMOUNT points of damage.
                        if (line.IndexOf(" reflect ") != -1)
                        {
                            lengthOfSource = line.IndexOf(" reflect shield ") - 2 - indexOfSource;
                            indexOfTarget = indexOfSource + lengthOfSource + 22;
                            amountType = "Reflect";
                        }
                        //SOURCE's damage shield hit TARGET for AMOUNT points of damage.
                        else
                        {
                            lengthOfSource = line.IndexOf(" damage shield ") - 2 - indexOfSource;
                            indexOfTarget = indexOfSource + lengthOfSource + 21;
                            amountType = "Shield";
                        }

                        lengthOfTarget = line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        source = line.Substring(indexOfSource, lengthOfSource);
                    }
                    //Something hit TARGET for AMOUNT points of damage by damage shield.
                    //Something hit TARGET for AMOUNT points of damage by reflect shield.
                    else
                    {
                        indexOfTarget = 14;
                        lengthOfTarget = line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        //Something hit TARGET for AMOUNT points of damage by damage shield.
                        if (line[line.Length - 9] == 'e')
                        {
                            amountType = "Shield";
                        }
                        //Something hit TARGET for AMOUNT points of damage by reflect shield.
                        else
                        {
                            amountType = "Reflect";
                        }

                    }

                    target = line.Substring(indexOfTarget, lengthOfTarget);
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    action = "Damage";

                    break;

                //TARGET was attacked with nanobots from SOURCE for AMOUNT points of AMOUNTTYPE damage.
                //TARGET was attacked with nanobots for AMOUNT points of AMOUNTTYPE damage.
                case "04":

                    indexOfTarget = 0;
                    lengthOfTarget = line.IndexOf(" was ");

                    //TARGET was attacked with nanobots from SOURCE for AMOUNT points of AMOUNTTYPE damage.
                    if (line.IndexOf(" from ") != -1)
                    {
                        indexOfSource = indexOfTarget + lengthOfTarget + 33;
                        lengthOfSource = line.IndexOf(" for ") - indexOfSource;
                        indexOfAmount = indexOfSource + lengthOfSource + 5;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        source = line.Substring(indexOfSource, lengthOfSource);

                    }
                    //TARGET was attacked with nanobots for AMOUNT points of AMOUNTTYPE damage.
                    else
                    {
                        indexOfAmount = indexOfTarget + lengthOfTarget + 32;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        //might not be true
                        source = "Unknown-Source";

                    }

                    indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                    lengthOfAmountType = line.Length - 8 - indexOfAmountType;

                    action = "Nano";
                    target = line.Substring(indexOfTarget, lengthOfTarget);
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    amountType = line.Substring(indexOfAmountType, lengthOfAmountType);

                    break;

                //You were healed for AMOUNT points.
                case "15":

                    indexOfAmount = 20;
                    lengthOfAmount = line.Length - 8 - indexOfAmount;

                    action = "Regen";
                    source = "userOfTheDamageMeter";
                    target = "userOfTheDamageMeter";
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    amountType = "Heal";

                    break;

                //You hit TARGET for AMOUNT points of AMOUNTTYPE damage.
                //Your damage shield hit TARGET for AMOUNT points of damage.
                case "08":

                    if (line.StartsWith("Your"))
                    {
                        indexOfTarget = 23;
                        lengthOfTarget = line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        action = "Damage";
                        source = "userOfTheDamageMeter";
                        target = line.Substring(indexOfTarget, lengthOfTarget);
                        amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                        amountType = "Shield";
                    }
                    else
                    {
                        indexOfTarget = 8;
                        lengthOfTarget = line.IndexOf(" for ") - indexOfTarget;

                        indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                        lengthOfAmountType = line.Length - 8 - indexOfAmountType;

                        action = "Damage";
                        source = "userOfTheDamageMeter";
                        target = line.Substring(indexOfTarget, lengthOfTarget);
                        amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                        amountType = line.Substring(indexOfAmountType, lengthOfAmountType);

                        //You hit TARGET for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                        if (line[line.Length - 1] == '!')
                        {
                            modifier = "Crit";
                        }
                        //You hit TARGET for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                        else if (line[line.Length - 2] == 't')
                        {
                            modifier = "Glance";
                        }
                    }

                    break;

                //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage.
                //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                //You absorbed AMOUNT points of AMOUNTTYPE damage.
                case "06":

                    //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage.
                    if (!line.StartsWith("You absorbed "))
                    {

                        indexOfSource = 0;
                        lengthOfSource = line.IndexOf(" hit ") - indexOfSource;

                        indexOfAmount = indexOfSource + lengthOfSource + 13;
                        lengthOfAmount = line.LastIndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                        lengthOfAmountType = line.Length - 8 - indexOfAmountType;

                        action = "Damage";
                        source = line.Substring(indexOfSource, lengthOfSource);

                        //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                        if (line[line.Length - 1] == '!')
                        {
                            lengthOfAmountType = line.Length - 22 - indexOfAmountType;
                            modifier = "Crit";
                        }
                        //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                        else if (line[line.Length - 2] == 't')
                        {
                            lengthOfAmountType = line.Length - 22 - indexOfAmountType;
                            modifier = "Glance";
                        }
                    }
                    //You absorbed AMOUNT points of AMOUNTTYPE damage.
                    else
                    {

                        indexOfAmount = 13;
                        lengthOfAmount = line.IndexOf(" points ") - indexOfAmount;

                        indexOfAmountType = indexOfAmount + 8;
                        lengthOfAmountType = line.Length - 8 - indexOfAmountType;

                        action = "Absorb";
                        source = "userOfTheDamageMeter";
                    }

                    target = "userOfTheDamageMeter";
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    amountType = line.Substring(indexOfAmountType, lengthOfAmountType);

                    break;

                //SOURCE tried to hit you, but missed!
                case "13":

                    indexOfSource = 0;
                    lengthOfSource = line.IndexOf(" tried ") - indexOfSource;

                    action = "Miss";
                    source = line.Substring(indexOfSource, lengthOfSource);
                    target = "userOfTheDamageMeter";

                    break;

                //You got nano from SOURCE for AMOUNT points.
                case "16":

                    indexOfSource = 18;
                    lengthOfSource = line.IndexOf(" for ") - indexOfSource;

                    indexOfAmount = indexOfSource + lengthOfSource + 5;
                    lengthOfAmount = line.Length - 8 - indexOfAmount;

                    action = "Heal";
                    source = line.Substring(indexOfSource, lengthOfSource);
                    target = "userOfTheDamageMeter";
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    amountType = "Nano";

                    break;

                //You tried to hit TARGET, but missed!
                case "12":


                    indexOfTarget = 17;
                    lengthOfTarget = line.Length - 13 - indexOfTarget;

                    action = "Miss";
                    source = "userOfTheDamageMeter";
                    target = line.Substring(indexOfTarget, lengthOfTarget);

                    break;

                //You increased nano on TARGET for AMOUNT points.
                case "17":

                    indexOfTarget = 22;
                    lengthOfTarget = line.LastIndexOf(" for ") - indexOfTarget;

                    indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                    lengthOfAmount = line.Length - 8 - indexOfAmount;

                    action = "Heal";
                    source = "userOfTheDamageMeter";
                    target = line.Substring(indexOfTarget, lengthOfTarget);
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    amountType = "Nano";

                    break;

                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                //SOURCE hit TARGET for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                case "09":

                    indexOfSource = 0;
                    lengthOfSource = line.IndexOf(" hit ") - indexOfSource;
                    // + 5 to skip the " hit " indices
                    indexOfTarget = lengthOfSource + 5;
                    lengthOfTarget = line.IndexOf(" for ") - indexOfTarget;
                    // + 5 to skip the " for " indices
                    indexOfAmount = indexOfTarget + lengthOfTarget + 5;
                    lengthOfAmount = line.LastIndexOf(" points ") - indexOfAmount;
                    // + 8 to skip the " points " indices, + 3 to skip the "of " indices
                    indexOfAmountType = indexOfAmount + lengthOfAmount + 11;
                    lengthOfAmountType = line.Length - 8 - indexOfAmountType;

                    //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Critical hit!
                    if (line[line.Length - 1] == '!')
                    {
                        lengthOfAmountType = line.Length - 22 - indexOfAmountType;
                        modifier = "Crit";
                    }
                    //SOURCE hit you for AMOUNT points of AMOUNTTYPE damage. Glancing hit.
                    else if (line[line.Length - 2] == 't')
                    {
                        lengthOfAmountType = line.Length - 22 - indexOfAmountType;
                        modifier = "Glance";
                    }

                    action = "PetDamage";
                    source = line.Substring(indexOfSource, lengthOfSource);
                    target = line.Substring(indexOfTarget, lengthOfTarget);
                    amount = Convert.ToInt32(line.Substring(indexOfAmount, lengthOfAmount));
                    amountType = line.Substring(indexOfAmountType, lengthOfAmountType);
                    source = line.Substring(indexOfSource, lengthOfSource);

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

/*******************************************************************
 * File : ITransaction.cs
 * Description :
 *      This file defines a basic transaction class.
 * 
 * Date : 2020.5.28
 *******************************************************************/

using System;

namespace BlockChainDemo
{
    /// <summary>
    /// A basic transaction type.</summary>
    public class Transaction : ITransaction
    {
        /// <summary>
        /// Address string from which coins transfer. </summary>
        private string from;

        /// <summary>
        /// Address string to which coins transfer. </summary>
        private string to;

        /// <summary>
        /// Amount of coins transferred.</summary>
        private ulong amount;

        // Constructor
        public Transaction(string from, string to, ulong amount)
        {
            this.from = from;
            this.to = to;
            this.amount = amount;
        }

        /// <summary>
        /// String that contains all transaction details. </summary>
        public string Summary()
        {
            return this.ToString();
        }

        /// <summary>
        /// Override the default <c>ToString</c> method to print transaction details.</summary>
        public override string ToString()
        {
            return "from:" + from + "\n" 
                + "to:" + to + "\n" 
                + "amount:" + amount;
        }
    }
}
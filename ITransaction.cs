/*******************************************************************
 * File : ITransaction.cs
 * Description :
 *      This file defines an inteface for transaction classes.
 * 
 * Date : 2020.5.28
 *******************************************************************/

namespace BlockChainDemo
{
    /// <summary>
    /// An interface for all possible transaction types. It contains only one method
    /// named <c>Summary</c>. By introducing this interface, I restrict that a 
    /// transaction types must be able to convert its transaction details into a 
    /// string. </summary>
    public interface ITransaction
    {
        /// <summary>
        /// returns a string that contains all transaction details.</summary>
        string Summary();
    }
}
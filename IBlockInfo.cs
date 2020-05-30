/*******************************************************************
 * File : IBlockInfo.cs
 * Description :
 *      This file defines an inteface for output block information.
 * 
 * Date : 2020.5.28
 *******************************************************************/


namespace BlockChainDemo
{
    /// <summary>
    /// An interface for all possible Block classes in a Block Chain. It contains 4 
    /// properties that define all information in a transaction Block, which include
    /// the transaction details (<c>Transaction</c>), the nonce (<c>Nonce</c>), the
    /// timestamp (<c>Timestamp</c>), and the hash key for the abover three elements
    /// (<c>Key</c>). </summary>
    public interface IBlockInfo
    {
        /// <summary>
        /// Transaction details corresponding to the Block.</summary>
        ITransaction Transaction { get; }

        /// <summary>
        /// Hash key for the Block.</summary>
        string Key { get; }

        /// <summary>
        /// Nonce for the Block.</summary>
        ulong Nonce { get; }

        /// <summary>
        /// Timestamp for the Block.</summary>
        long Timestamp { get; }
    }
}
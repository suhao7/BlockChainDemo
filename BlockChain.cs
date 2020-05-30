/*******************************************************************
 * File : BlockChain.cs
 * Description :
 *      This file implements a block chain demo.
 * 
 * Date : 2020.5.28
 *******************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockChainDemo
{
    /// <summary>
    /// A simple Block Chain implement</summary>
    public class BlockChain : IEnumerable<IBlockInfo>
    {
        /// <summary>
        /// Mining reward is fixed to be 5 coins per block</summary>
        public readonly uint minereward = 5;
        
        /// <summary>
        /// stores transactions to be appended to the Block Chain</summary>
        private List<ITransaction> transactionPool;

        /// <summary>
        /// Number of blocks</summary>
        private int N;

        /// <summary>
        /// the latest appended block, representing the lastest transaction</summary>
        private Block current;
        
        /// <summary>
        /// internal class that defines the <c>Block</c>.</summary>
        private class Block
        {
            internal readonly string difficulty = "000"; // fixed mining difficulty
            internal string key; // SHA256 key
            internal ITransaction transaction; // transaction details
            internal Block previous; // reference of last Block
            internal ulong nonce; // nonce to be calculated when mining
            internal long timestamp; // timestamp, in utc file time format
            

            public Block(ITransaction transaction, Block previous)
            {
                this.timestamp = DateTime.Now.ToFileTimeUtc();
                this.transaction = transaction;
                this.previous = previous;
                this.nonce = 1;
                Mine();
            }

            // mining the block, i.e., calculate the nonce.
            private void Mine()
            {
                while (true)
                {
                    // first compute the key
                    if (previous == null)
                        key = ComputeHash(transaction.Summary() +
                            timestamp.ToString() + 
                            nonce.ToString());
                    else
                        key = ComputeHash(transaction.Summary() + 
                            timestamp.ToString() + 
                            previous.key + 
                            nonce.ToString());
                    
                    // test if key meet difficulty requirement, if not, continue to mine.
                    if (key.Substring(0, difficulty.Length) == difficulty)
                        break;
                    else
                        nonce++;
                }
            }

            // return summary information of this block.
            public IBlockInfo Summary()
            {
                return new BlockInfo(transaction, timestamp, nonce, key);
            }
        }

        /// <summary>
        /// internal class that implements <c>IBlockInfo</c> interface, defining the block information 
        /// available to outside <c>BlockChain</c> users</summary>
        public class BlockInfo : IBlockInfo
        {
            public ITransaction Transaction { get; }
            public string Key { get; }
            public ulong Nonce { get; }
            public long Timestamp { get; }
            public BlockInfo(ITransaction transaction, long timestamp, ulong nonce, string key)
            {
                Transaction = transaction;
                Key = key;
                Nonce = nonce;
                Timestamp = timestamp;
            }
            public override string ToString()
            {
                return "Transaction:" + Transaction.Summary() + "\n" +
                    "Timestamp:" + Timestamp + "\n" +
                    "Nonce:" + Nonce + "\n" +
                    "Key:" + Key;
            }
        }

        /// <summary>
        /// constructor that generates an empty Block Chain
        /// </summary>
        public BlockChain()
        {
            transactionPool = new List<ITransaction>();
        }

        /// <summary>
        /// Number of blocks</summary>
        public int Count { get => N; } 

        /// <summary>
        /// whether the chain is empty</summary>
        public bool IsEmpty { get => N == 0; } 

        /// <summary>
        /// Return true if the given trade key can be found in this chain.
        /// </summary>
        /// <returns><c>true</c> if <c>key</c> is contained in this chain.
        ///     <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNUllException">if <c>key</c> is <c>null</c></exception>
        public bool Contains(string key)
        {
            if (key == null)
                throw new ArgumentNullException("argument to Contains() is null");
            return Get(key) != null;
        }

        /// <summary>
        /// Index wrapping <c>Get</c> for convenient syntax.
        /// </summary>
        public IBlockInfo this[string key]
        {
            get
            {
                object value = Get(key);
                if (value == null)
                {
                    if (default(IBlockInfo) == null)
                        return (IBlockInfo)value;
                    else 
                        throw new NullReferenceException("null reference being used for value type");
                }
                return (IBlockInfo)value;
            }
        }

        /// <summary>
        /// Return corresponding <c>BlockInfo</c> if the given trade key can be found in this chain.
        /// </summary>
        /// <returns><c>BlockInfo</c> if <c>key</c> is contained in this chain.
        ///     <c>null</c> otherwise.</returns>
        /// <exception cref="ArgumentNUllException">if <c>key</c> is <c>null</c></exception>
        public object Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException("argument to Get() is null");
            for (Block x = current; x != null; x = x.previous)
            {
                if (key.Equals(x.key))
                    return x.Summary();
            }
            return null;
        }

        /// <summary>
        /// Add a transaction to the chain</summary>
        private void AddToChain(ITransaction transaction)
        {
            if (transaction == null) // do nothing.
                return;
            current = new Block(transaction, current);
            N++;
        }

        /// <summary>
        /// Add a transaction to the <c>transactionPool</c> </summary>
        public void AddToPool(ITransaction transaction)
        {
            if (transaction == null)
                return;
            transactionPool.Add(transaction);
        }

        /// <summary>
        /// Add a series of transactions to the <c>transactionPool</c> </summary>
        public void AddToPool(IEnumerable<ITransaction> transactions)
        {
            if (transactions == null)
                return;
            transactionPool.AddRange(transactions);
        }

        /// <summary>
        /// Returns all transactions to be appended to the chain as an <c>IEnumerable</c>. </summary>
        public IEnumerable<ITransaction> ViewPool()
        {
            return (IEnumerable<ITransaction>)transactionPool;
        }

        /// <summary>
        /// Returns all block keys in the chain as an <c>IEnumerable</c>. </summary>
        public IEnumerable<string> Keys()
        {
            for (Block x = current; x != null; x = x.previous)
                yield return x.key;
        }

        public IEnumerator<IBlockInfo> GetEnumerator()
        {
            for (Block x = current; x != null; x = x.previous)
                yield return x.Summary();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Mine the transaction pool, clear the pool, and finally reward the miner. </summary>
        public void MineTransactionPool()
        {
            uint n = 0;
            foreach (ITransaction t in transactionPool)
            {
                AddToChain(t);
                n++;
            }
            transactionPool.Clear();
            AddToChain(new Transaction("", "localhost", (ulong)minereward*n));
        }

        /// <summary>
        /// Calculate SHA256 key for input string <c>s</c>, 
        /// and return the key as hex string. </summary>
        public static string ComputeHash(string s)
        {
            return BitConverter.ToString(
                    SHA256Managed.Create().ComputeHash(
                        Encoding.ASCII.GetBytes(s)))
                    .Replace("-","");
        }

        /// <summary>
        /// Return true if the given block chain is altered. False otherwise.
        /// </summary>
        /// <returns><c>true</c> if <c>chain</c> is not altered.
        ///     <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNUllException">if <c>chain</c> is <c>null</c></exception>
        public static bool ValidateChain(BlockChain chain)
        {
            if (chain == null)
                throw new ArgumentNullException("argument to ValidateChain() is null");

            for (Block x = chain.current; x != null; x = x.previous)
            {
                if (x.previous != null)
                {
                    if (ComputeHash(x.transaction.Summary() + 
                            x.timestamp.ToString() + 
                            x.previous.key + 
                            x.nonce.ToString()) != x.key) 
                        return false;
                }
                else
                {
                    if (ComputeHash(x.transaction.ToString() +
                            x.timestamp.ToString() +
                            x.nonce.ToString()) != x.key) 
                        return false;
                }
            }
            return true;
        }
        
        public static void Test()
        {
            // Initialize a block chain, and print its size.
            var bc = new BlockChain();
            Console.WriteLine("The number of Blocks: {0}", bc.Count);
            Console.WriteLine("The Chain is empty? {0}", bc.IsEmpty);

            // Add transactions to transaction pool of this chain.
            bc.AddToPool(new Transaction("Potter", "Weasleys", 120));
            bc.AddToPool(new Transaction("Copperfield", "Micawber", 200));
            bc.AddToPool(new Transaction("Magwitch", "Pip", 150));
            bc.AddToPool(new Transaction("Appel", "Henry", 100));
            Console.WriteLine("The number of Blocks: {0}", bc.Count);

            // Print the transaction pool.
            foreach (ITransaction x in bc.ViewPool())
                Console.WriteLine(x);

            // Mine the pool.
            bc.MineTransactionPool();
            Console.WriteLine("The number of Blocks: {0}", bc.Count);

            // Print the transaction pool again.
            foreach (ITransaction x in bc.ViewPool())
                Console.WriteLine(x);

            // Print block keys.
            var keys = bc.Keys();
            var klist = new List<string>();
            foreach (var k in keys)
            {
                Console.WriteLine(k);
                klist.Add(k);
            }

            // Validate the chain.
            Console.WriteLine(ValidateChain(bc));

            // Obtain corresponding block information from the key.
            string sk = klist[0];
            Console.WriteLine(bc.Get(sk));

            // Prove that key is unique.
            Console.WriteLine(bc.Contains(sk));
            Console.WriteLine(bc.Contains(sk.Replace("C","D")));

            // Print summary information of blocks.
            foreach (var x in bc)
                Console.WriteLine(x);
        }
    }
}
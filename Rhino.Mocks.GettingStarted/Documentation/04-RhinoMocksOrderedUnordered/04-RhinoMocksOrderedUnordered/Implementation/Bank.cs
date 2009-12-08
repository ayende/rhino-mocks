using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhinoMocksIntroduction
{
    /// <summary>
    /// Minimum implementation to pass the test
    /// </summary>
    public class Bank
    {
        IDatabaseManager m_DatabaseManager;

        public Bank(IDatabaseManager dabaseManager)
        {
            m_DatabaseManager = dabaseManager;
        }

        public void TransferFunds(IBankAccount accountOne, IBankAccount accountTwo, int p)
        {
            m_DatabaseManager.BeginTransaction();         
            accountTwo.Deposit(p);
            accountOne.Withdraw(p);            
            m_DatabaseManager.Dispose();        
        }
    }
}

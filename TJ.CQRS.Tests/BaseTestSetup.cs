using System;
using System.Collections.Generic;
using NUnit.Framework;
using TJ.CQRS.Messaging;
using TJ.CQRS.Tests.Stub;
using TJ.Extensions;

namespace TJ.CQRS.Tests
{
    [TestFixture]
    public abstract class BaseTestSetup
    {
        private Exception _caughtException;
        private bool _exceptionIsChecked;
        private bool _exceptionOccured;

        protected Exception CaughtException
        {
            get
            {
                _exceptionIsChecked = true;
                return _caughtException ?? new Exception();
            }
        }

        protected abstract void Given();

        [TestFixtureSetUp]
        public void Setup()
        {
            _exceptionOccured = false;
            _exceptionIsChecked = false;
            try
            {
                Given();
            }
            catch (Exception ex)
            {
                _exceptionOccured = true;
                _caughtException = ex;
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            if (_exceptionOccured && _exceptionIsChecked.IsFalse())
            {
                throw _caughtException;
            }            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TJ.CQRS.Exceptions;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.Tests
{
    [TestFixture]
    public class When_Executing_An_Unregistered_Command
    {
        private InMemoryBus _inMemoryBus;

        [TestFixtureSetUp]
        public void Setup()
        {
            _inMemoryBus = new InMemoryBus(new MessageRouter());
        }

        [Test]
        public void Then_An_Unregistered_Command_Exception_Should_Be_Thrown()
        {
            // Assert
            Action act = () => _inMemoryBus.Send(new StubCommand());
            act.ShouldThrow<UnregisteredCommandException>();
        }
    }

    [TestFixture]
    public class When_Executing_A_Registered_Command
    {
        private InMemoryBus _inMemoryBus;
        private StubCommandHandler _commandHandler1;
        private StubCommandHandler _commandHandler2;
        private StubCommand _command;
        private StubUnitOfWork _unitOfWork;

        [TestFixtureSetUp]
        public void Setup()
        {
            _commandHandler1 = new StubCommandHandler();
            _commandHandler2 = new StubCommandHandler();
            var messageRouter = new MessageRouter();
            messageRouter.Register<StubCommand>(_commandHandler1.Handle);
            messageRouter.Register<StubCommand>(_commandHandler2.Handle);
            _unitOfWork = new StubUnitOfWork();
            _inMemoryBus = new InMemoryBus(messageRouter);
            _inMemoryBus.Commit += _unitOfWork.Commit;
            _command = new StubCommand();
            _inMemoryBus.Send(_command);
        }

        [Test]
        public void Then_All_The_Command_Should_Be_Executed()
        {
            _commandHandler2.ExecutedCommand.Should().BeSameAs(_command);
            _commandHandler1.ExecutedCommand.Should().BeSameAs(_command);
        }

        [Test]
        public void And_The_Changes_Should_Be_Persisted()
        {
            _unitOfWork.CommitCount.Should().Be(1);
        }
    }

    public class StubUnitOfWork : IUnitOfWork
    {
        private int _commitCount = 0;
        private decimal _undoChangesCount;

        public int CommitCount
        {
            get { return _commitCount; }
        }

        public decimal UndoChangesCount
        {
            get { return _undoChangesCount; }
        }

        public void Rollback()
        {
            _undoChangesCount++;
        }

        public void Commit()
        {
            _commitCount++;
        }
    }

    public class StubCommandHandler : IHandle<StubCommand>
    {
        public StubCommand ExecutedCommand { get; set; }

        public void Handle(StubCommand command)
        {
            ExecutedCommand = command;
        }
    }

    public class StubCommand : Messaging.Command
    {
        public StubCommand()
            : base(Guid.NewGuid())
        {

        }
    }
}

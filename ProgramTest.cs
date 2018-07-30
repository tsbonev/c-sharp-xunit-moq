using System;
using System.IO;
using Moq;
using Xunit;

namespace XUnitTests
{

    public interface IMoveable
    {
        int move();
        int moveTo(int a);
        int moveExactlyTo(int a);
        void throwException();
    }

    public class Box
    {
        public IMoveable Moveable;

        public void throwException()
        {
            throw new Exception();
        }

        public Box(IMoveable moveable)
        {
            this.Moveable = moveable;
        }
    }

    public class ProgramTest : IDisposable
    {
        private MemoryStream _stream;

        //Class constructor servers as Before each
        public ProgramTest()
        {
            Console.WriteLine("Before");
            _stream = new MemoryStream();
        }

        //Class dispose serves as After each
        public void Dispose()
        {
            Console.WriteLine("After");
            _stream.Dispose();
        }

        [Fact]
        public void MultipleTests()
        {
            //Assert boolean value
            //Stream is open before each test
            Assert.True(_stream.CanRead);
        }

        [Fact]
        public void TestMock()
        {
            //Creating a mock
            //Strict prohibits calls that haven't been set up
            var moveable = new Mock<IMoveable>(MockBehavior.Strict);

            //Setting up an expectation
            //Verifiable records calls
            moveable.Setup(m => m.move()).Returns(2).Verifiable();
            moveable.Setup(m => m.moveTo(2)).Returns(4).Verifiable();

            //Setting up an expectation with any parameter
            moveable.Setup(m => m.moveExactlyTo(It.IsAny<Int32>())).Returns(8).Verifiable();

            //Setting up a throw mock
            moveable.Setup(m => m.throwException()).Throws<Exception>().Verifiable();

            //Passing mock as an object of its interface
            Box box = new Box(moveable.Object);
            
            //Asserting equality
            Assert.Equal(2, box.Moveable.move());
            Assert.Equal(2, box.Moveable.move());
            Assert.Equal(4, box.Moveable.moveTo(2));
            Assert.Equal(8, box.Moveable.moveExactlyTo(1234));

            //Asserting exceptions
            Assert.Throws<Exception>(() => box.throwException());
            Assert.Throws<Exception>(() => box.Moveable.throwException());

            //Verifying specific occurances
            moveable.Verify(m => m.move(), Times.AtLeast(2));

            //Verifying any occurance
            moveable.Verify();

        }
    }
}

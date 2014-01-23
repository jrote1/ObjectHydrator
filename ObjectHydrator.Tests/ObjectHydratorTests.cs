﻿using System.Collections.Generic;
using System.Data;
using Moq;
using NUnit.Framework;
using SqlObjectHydrator.Configuration;
using SqlObjectHydrator.Tests.TestData;

namespace SqlObjectHydrator.Tests
{
    [TestFixture]
    public class ObjectHydratorTests
    {
        [Test]
        public void DataReaderToList_WhenCalled_ReturnsInstance()
        {
            var dataReader = new Mock<IDataReader>();

            var objectHydrator = new ObjectHydrator();

            var result = objectHydrator.DataReaderToList<User>( dataReader.Object );

            Assert.IsInstanceOf<List<User>>( result );
        }

        [Test]
        public void DataReaderToList_WhenCalled_ReturnsCorrectData()
        {
            var rows = 0;
            var dataReader = new Mock<IDataReader>();
            dataReader.SetupGet( x => x.FieldCount ).Returns( 2 );
            dataReader.Setup( x => x.GetName( 0 ) ).Returns( "Id" );
            dataReader.Setup( x => x.GetFieldType( 0 ) ).Returns( typeof ( int ) );
            dataReader.Setup( x => x.GetName( 1 ) ).Returns( "FullName" );
            dataReader.Setup( x => x.GetFieldType( 1 ) ).Returns( typeof ( string ) );
            dataReader.Setup( x => x.GetInt32( 0 ) ).Returns( () => rows );
            dataReader.Setup( x => x.GetString( 1 ) ).Returns( () => "Name" + rows );

            dataReader.Setup( x => x.Read() ).Returns( () => ++rows <= 2 );


            var objectHydrator = new ObjectHydrator();

            var result = objectHydrator.DataReaderToList( dataReader.Object, new ObjectHydratorConfiguration<User>()
                                                                                       .Mapping( x => x.RefId, x => x.GetInt32( 0 ) * 2 ) );

            Assert.AreEqual( 2, result.Count );
            Assert.AreEqual( 1, result[ 0 ].Id );
            Assert.AreEqual( "Name1", result[ 0 ].FullName );
            Assert.AreEqual( 2, result[ 0 ].RefId );
            Assert.AreEqual( 2, result[ 1 ].Id );
            Assert.AreEqual( "Name2", result[ 1 ].FullName );
            Assert.AreEqual( 4, result[ 1 ].RefId );
        }

        //[Test]
        //public void DataReaderToList_WhenCalledWithInvalidConfiguration_ThrowsException()
        //{
        //    var dataReader = new Mock<IDataReader>();
        //    dataReader.SetupGet( x => x[ 0 ] ).Returns( "Hello World" );

        //    var configuration = new ObjectHydratorConfiguration<User>()
        //        .Mapping( x => x.Id, x => x[ 0 ] );

        //    var objectHydrator = new ObjectHydrator();

        //    Assert.Throws<Exception>( () => objectHydrator.DataReaderToList( dataReader.Object, configuration ) );
        //}
    }
}
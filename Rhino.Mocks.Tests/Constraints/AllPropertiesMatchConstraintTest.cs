using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using MbUnit.Framework;

using Rhino.Mocks.Constraints;

#region TestObjects
namespace Rhino.Mocks.Tests.AllPropertiesMatchConstraint
{
    public class Catalog
    {
        private Dictionary<string, Product> _products;

        public Catalog()
        {
            _products = new Dictionary<string, Product>();
        }

        public Dictionary<string, Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }
    }

    public class Order
    {
        private Product _product;
        private int _quantity;
        private bool _isFilled = false;


        public Product Product
        {
            get { return _product; }
            set { _product = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public bool IsFilled
        {
            get { return _isFilled; }
            set { _isFilled = value; }
        }
    }

    public class Product
    {
        string _name;
        double _price;
        int? _weight;

        public Product(string name, double price)
        {
            _name = name;
            _price = price;
        }

        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int? Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
    }

    class ShippingList
    {
        private List<Product> _products;
        public DateTime _shippingDate;

        public ShippingList()
        {
            _products = new List<Product>();
        }

        public List<Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }
    }
}

#endregion

namespace Rhino.Mocks.Tests.Constraints
{
	using System.Globalization;
	using System.Threading;
	using Rhino.Mocks.Tests.AllPropertiesMatchConstraint;

    [TestFixture]
    public class AllPropertiesMatchConstraintTest
    {
    	private CultureInfo old;

    	[SetUp]
    	public void SetUp()
    	{
    		old = Thread.CurrentThread.CurrentCulture;
    		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    	}

    	[TearDown]
    	public void TearDown()
    	{
    		Thread.CurrentThread.CurrentCulture = old;
    	}

        [Test]
        public void SuccessTest()
        {
            Order order = new Order();
            order.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            order.Quantity = 4;

            Order expectedOrder = new Order();
            expectedOrder.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            expectedOrder.Quantity = 4;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsTrue(sut.Eval(order));
        }

        [Test]
        public void ValueTypePropertyNotEqual()
        {
            Order order = new Order();
            order.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            order.Quantity = 4;

            Order expectedOrder = new Order();
            expectedOrder.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            expectedOrder.Quantity = 10;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsFalse(sut.Eval(order));
            Assert.AreEqual("Expected value of Order.Quantity is '10', actual value is '4'", sut.Message);
        }
        
        [Test]
        public void NestedPropertyNotEqual()
        {
            Order order = new Order();
            order.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            order.Quantity = 4;

            Order expectedOrder = new Order();
            expectedOrder.Product = new Product("Ratched and Clank - Tools of Destruction", 50.0);
            expectedOrder.Quantity = 4;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsFalse(sut.Eval(order));
            Assert.AreEqual("Expected value of Order.Product.Price is '50', actual value is '61.05'", sut.Message);
        }

        [Test]
        public void ReferenceTypePropertyNullTest()
        {
            Order order = new Order();
            order.Product = null;
            order.Quantity = 4;

            Order expectedOrder = new Order();
            expectedOrder.Product = null;
            expectedOrder.Quantity = 4;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsTrue(sut.Eval(order));
        }

        [Test]
        public void ExpectedReferenceTypePropertyNullTest()
        {
            Order order = new Order();
            order.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            order.Quantity = 4;

            Order expectedOrder = new Order();
            expectedOrder.Product = null;
            expectedOrder.Quantity = 4;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsFalse(sut.Eval(order));
            Assert.AreEqual("Expected value of Order.Product is null, actual value is 'Rhino.Mocks.Tests.AllPropertiesMatchConstraint.Product'", sut.Message);
        }

        [Test]
        public void ActualReferenceTypePropertyNullTest()
        {
            Order order = new Order();
            order.Product = null;
            order.Quantity = 4;

            Order expectedOrder = new Order();
            expectedOrder.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            expectedOrder.Quantity = 4;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsFalse(sut.Eval(order));
            Assert.AreEqual("Expected value of Order.Product is 'Rhino.Mocks.Tests.AllPropertiesMatchConstraint.Product', actual value is null", sut.Message);
        }

        [Test]
        public void NullableTypeSetTest()
        {
            Order order = new Order();
            order.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            order.Quantity = 4;
            order.Product.Weight = 18;

            Order expectedOrder = new Order();
            expectedOrder.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            expectedOrder.Quantity = 4;

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedOrder);

            Assert.IsFalse(sut.Eval(order));
            Assert.AreEqual("Expected value of Order.Product.Weight is null, actual value is '18'", sut.Message);
        }

        [Test]
        public void ExpectedDifferentTypeThanActual()
        {
            Order order = new Order();
            order.Product = new Product("Ratched and Clank - Tools of Destruction", 61.05);
            order.Quantity = 4;
            order.Product.Weight = 18;

            Product expectedProduct = new Product("Ratched and Clank - Tools of Destruction", 61.05);

            AbstractConstraint sut = Property.AllPropertiesMatch(expectedProduct);

            Assert.IsFalse(sut.Eval(order));
            Assert.AreEqual("Expected type 'Product' doesn't match with actual type 'Order'", sut.Message);
        }

        [Test]
        public void SimpleReferenceTypeSuccess()
        {
            string actual = "hello world.";
            string expected = "hello world.";

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsTrue(sut.Eval(actual));
        }

        [Test]
        public void SimpleReferenceTypeFail()
        {
            string actual = "hello world.";
            string expected = "hello wonderfull world.";

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("Expected value of String is 'hello wonderfull world.', actual value is 'hello world.'", sut.Message);
        }

        [Test]
        public void CollectionPropertyTest()
        {
            ShippingList actual = new ShippingList();
            actual.Products.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));
            actual.Products.Add(new Product("Assassin's Creed", 69.99));

            ShippingList expected = new ShippingList();
            expected.Products.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));
            expected.Products.Add(new Product("Uncharted - Drake's Fortune", 69.99));

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("Expected value of ShippingList.Products[1].Name is 'Uncharted - Drake's Fortune', actual value is 'Assassin's Creed'", sut.Message);
        }

        [Test]
        public void CollectionPropertyCountTest()
        {
            ShippingList actual = new ShippingList();
            actual.Products.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));

            ShippingList expected = new ShippingList();
            expected.Products.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));
            expected.Products.Add(new Product("Uncharted - Drake's Fortune", 69.99));

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("expected number of items in collection ShippingList.Products is '2', actual is '1'", sut.Message);
        }

        [Test]
        public void CollectionActualCountTest()
        {
            List<Product> actual = new List<Product>();
            actual.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));

            List<Product> expected = new List<Product>();
            expected.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));
            expected.Add(new Product("Uncharted - Drake's Fortune", 69.99));

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("expected number of items in collection List`1 is '2', actual is '1'", sut.Message);
        }

        [Test]
        public void CollectionExpectedCountTest()
        {
            List<Product> actual = new List<Product>();
            actual.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));
            actual.Add(new Product("Uncharted - Drake's Fortune", 69.99));

            List<Product> expected = new List<Product>();
            expected.Add(new Product("Ratched and Clank - Tools of Destruction", 61.05));


            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("expected number of items in collection List`1 is '1', actual is '2'", sut.Message);
        }

        [Test]
        public void PublicFieldTest()
        {
            ShippingList actual = new ShippingList();
            actual._shippingDate = new DateTime(2007, 9, 27);

            ShippingList expected = new ShippingList();
            expected._shippingDate = new DateTime(1978, 9, 27);

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
			Assert.AreEqual("Expected value of ShippingList._shippingDate is '09/27/1978 00:00:00', actual value is '09/27/2007 00:00:00'", sut.Message);
        }

        [Test]
        public void DictionaryPropertyTest()
        {
            Catalog actual = new Catalog();
            actual.Products.Add("RC01", new Product("Ratched and Clank - Tools of Destruction", 61.05));
            actual.Products.Add("UDF1", new Product("Assassin's Creed", 69.99));

            Catalog expected = new Catalog();
            expected.Products.Add("RC01", new Product("Ratched and Clank - Tools of Destruction", 61.05));
            expected.Products.Add("UDF1", new Product("Uncharted - Drake's Fortune", 69.99));

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);
            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("Expected value of Catalog.Products[1].Value.Name is 'Uncharted - Drake's Fortune', actual value is 'Assassin's Creed'", sut.Message);
        }

        /// <summary>
        /// The mother of all tests, a DataRow object is mega complex with recusive properties
        /// and properties that give exceptions when the Get is invoked.
        /// </summary>
        /// <remarks>
        /// It's better to check DataRow's with a seperate specialized constraint but we want to
        /// be sure our constraint won't break if someone used it with a DataRow.
        /// </remarks>
        [Test]
        public void DataRowTest()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("id", typeof(int)));
            table.Columns.Add(new DataColumn("name", typeof(string)));

            DataRow actual = table.NewRow();
            actual["id"] = 1;
            actual["name"] = "Ratched and Clank - Tools of Destruction";

            DataRow expected = table.NewRow();
            expected["id"] = 2;
            expected["name"] = "Ratched and Clank - Tools of Destruction";

            AbstractConstraint sut = Property.AllPropertiesMatch(expected);

            Assert.IsFalse(sut.Eval(actual));
            Assert.AreEqual("Expected value of DataRow.ItemArray[0] is '2', actual value is '1'", sut.Message);
        }
    }
}

using System.Collections.Generic;
using NUnit.Framework;

public class BinaryHeapTest
{

	[Test]
	public void SimpleTest()
    {
        var heap = new BinaryHeap<int>(1024);
        var testCollection = new List<int>(new int[]{ 1, 3, 4, 2 });

        foreach(var item in testCollection)
        {
            heap.Add(item);
            System.Console.WriteLine(heap);
        }

        Assert.AreEqual(testCollection.Count, heap.Count);

        testCollection.Sort();

        foreach (var item in testCollection)
        {
            var gotItem = heap.Take();
            Assert.AreEqual(gotItem, item);
        }
	}
}

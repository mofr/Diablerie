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
            System.Console.Write(heap);
        }

        Assert.AreEqual(testCollection.Count, heap.Count);

        testCollection.Sort();

        foreach (var item in testCollection)
        {
            var gotItem = heap.Take();
            System.Console.Write(heap);
            Assert.AreEqual(gotItem, item);
        }
	}

    [Test]
    public void StressTest()
    {
        var heap = new BinaryHeap<int>(128);
        var testCollection = new List<int>();
        const int TrialCount = 100;
        var random = new System.Random(666);

        for (int trial = 0; trial < TrialCount; ++trial)
        {
            testCollection.Clear();
            heap.Clear();

            for(int i = 0; i < heap.MaxSize; ++i)
            {
                testCollection.Add(random.Next());
            }

            foreach (var item in testCollection)
            {
                heap.Add(item);
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
}

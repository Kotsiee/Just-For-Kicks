using System.Collections.Generic;

public abstract class AbstarctWeightedList<TItem, TWeight> { }

public class WeightedList<TItem, TWeight> : AbstarctWeightedList<TItem, TWeight>
{
    private List<TItem> items = new List<TItem>();
    private List<TWeight> weights = new List<TWeight>();

    private int count = 0;

    public List<TItem> getItems() { return items; }
    public TItem GetItem(int index) { return items[index]; }
    public List<TWeight> getWeights() { return weights; }
    public TWeight GetWeight(int index) { return weights[index]; }
    public int getCount() { return count; }

    public void add(TItem item, TWeight weight)
    {
        this.items.Add(item);
        this.weights.Add(weight);
        count++;
    }

    public void remove(TItem item)
    {
        if (count > 0)
        {
            if (count > 1)
            {
                int counter = 0;
                bool found = false;
                foreach (TItem i in items)
                {
                    if (found)
                    {
                        items[counter - 1] = items[counter];
                        weights[counter - 1] = weights[counter];
                    }



                    if (i.Equals(item))
                    {
                        items[counter] = default(TItem);
                        weights[counter] = default(TWeight);
                        found = true;
                    }
                }
            }
            else
                clear();

            count--;
        }
    }

    public void clear()
    {
        count = 0;
        items = null;
        weights = null;
    }

    public TItem getRandomItem()
    {
        float totalWeights = 0;

        foreach (TWeight weight in weights)
            totalWeights += float.Parse(weight.ToString());

        System.Random rnd = new System.Random();
        float randomWeight = (float)rnd.NextDouble() * totalWeights;

        float currentTotalWeight = 0;
        int pointer = 0;

        for (int i = 0; i < count; i++)
        {
            if (currentTotalWeight < randomWeight)
                pointer = i;

            currentTotalWeight += float.Parse(weights[i].ToString());
        }

        return items[pointer];
    }
}
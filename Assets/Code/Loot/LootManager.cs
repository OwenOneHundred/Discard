using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LootManager : MonoBehaviour
{
    public float commonWeight;
    public List<Loot> commonCards;
    public float uncommonWeight;
    public List<Loot> uncommonCards;
    public float rareWeight;
    public List<Loot> rareCards;

    private Dictionary<Rarity, List<Loot>> lootLists = new Dictionary<Rarity, List<Loot>>();

    private void Start()
    {
        lootLists.Add(Rarity.common, commonCards);
        lootLists.Add(Rarity.uncommon, uncommonCards);
        lootLists.Add(Rarity.rare, rareCards);
    }

    public Card GetLootCardOfRarity(Rarity rarity)
    {
        List<Loot> list = lootLists[rarity];
        return list[GeneralUtil.RandomWeighted(list.Select(x => x.oddsWeight).ToList())].cardSO;
    }

    public Card GetLootCardBySpecifiedWeights(float commonWeight1, float uncommonWeight1, float rareWeight1)
    {
        List<Loot> selectedList;
        switch (GeneralUtil.RandomWeighted(new List<float>() { commonWeight1, uncommonWeight1, rareWeight1 }))
        {
            case 0: selectedList = commonCards; break;
            case 1: selectedList = uncommonCards; break;
            case 2: selectedList = rareCards; break;
            default: selectedList = commonCards; break;
        }

        return selectedList[GeneralUtil.RandomWeighted(selectedList.Select(x => x.oddsWeight).ToList())].cardSO;
    }

    public Card GetLootCard()
    {
        List<Loot> selectedList;
        switch (GeneralUtil.RandomWeighted(new List<float>() { commonWeight, uncommonWeight, rareWeight }))
        {
            case 0: selectedList = commonCards; break;
            case 1: selectedList = uncommonCards; break;
            case 2: selectedList = rareCards; break;
            default: selectedList = commonCards; break;
        }

        return selectedList[GeneralUtil.RandomWeighted(selectedList.Select(x => x.oddsWeight).ToList())].cardSO;
    }

    public enum Rarity
    {
        common, uncommon, rare
    }

    [System.Serializable]
    public class Loot
    {
        public Card cardSO;
        public float oddsWeight = 1;
    }
}

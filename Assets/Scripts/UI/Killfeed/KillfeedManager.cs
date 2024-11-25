using DesignPatterns.Singleton;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kilfeed
{
    public class KillfeedManager : NetworkSingleton<KillfeedManager>
    {
        [SerializeField] private Transform container;
        [SerializeField] private KillfeedItem itemPrefab;

        string [] randomPhrases = { "slaughtered", "destroyed", "killed","demolished","erased" };

        public override KillfeedManager GetInstance()
        {
            return this;
        }


        [ClientRpc]
        public void RpcDisplayFeed(string killer, string phrase, string victim)
        {
            var item = Instantiate(itemPrefab);
            item.Construct(killer, phrase, victim);
            item.transform.SetParent(container);
        }

        public string GetPhrase() => randomPhrases[Random.Range(0, randomPhrases.Length)];
    }
}
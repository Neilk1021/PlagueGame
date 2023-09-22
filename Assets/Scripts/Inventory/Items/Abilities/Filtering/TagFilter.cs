using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "Tag Filter", menuName = "Items/Abilities/Filters/Tag")]
    public class TagFilter : FilterStrategy
    {
        [SerializeField] string tagToFilter = "";
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            return objectsToFilter.Where(objectF => objectF.tag == tagToFilter);
        }
    }
}

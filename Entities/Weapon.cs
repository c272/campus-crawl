using CampusCrawl.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusCrawl.Entities
{
    internal abstract class Weapon : Entity
    {
        protected string name;
        protected int range;
        protected int damage;
        protected string playerModel;
        protected string restingModel;
        protected string attackedModel;
        public Weapon(Character character, string playerModel, string assetPath) : base(assetPath) {
            this.playerModel = playerModel;
        }
        public abstract void Attack();
    }
}

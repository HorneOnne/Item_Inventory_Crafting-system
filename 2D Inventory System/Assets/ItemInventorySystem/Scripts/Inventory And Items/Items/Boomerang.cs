using UnityEngine;

namespace DIVH_InventorySystem
{
    /// <summary>
    /// A class representing a boomerang item in the game.
    /// </summary>
    public class Boomerang : Item, ICanCauseDamage
    {
        //private GameObject boomerangProjectilePrefab;
        private BoomerangProjectile_001 boomerangProjectileObject;

        private bool isReturning = true;
        private BoomerangData boomerangData;

        private void Awake()
        {
            //boomerangProjectilePrefab = GameDataManager.Instance.GetProjectilePrefab(boomerangProjectileObject);
        }


        private void OnEnable()
        {
            EventManager.OnBoomerangReturned += ResetBoomerangState;
        }

        private void OnDisable()
        {
            EventManager.OnBoomerangReturned -= ResetBoomerangState;
        }

        protected override void Start()
        {
            base.Start();
            
        }



        public override bool Use(Player player)
        {
            boomerangData = (BoomerangData)this.ItemData;
            /*if (boomerangProjectilePrefab == null)
                boomerangProjectilePrefab = BoomerangProjectileSpawner.Instance.prefab;*/

            switch (boomerangData.attackType)
            {
                case 1:
                    if (isReturning)
                    {
                        isReturning = false;                   
                        boomerangProjectileObject = BoomerangProjectileSpawner.Instance.Pool.Get().GetComponent<BoomerangProjectile_001>();
                        //boomerangProjectileObject = Instantiate(boomerangProjectilePrefab, transform.position, Quaternion.identity).GetComponent<BoomerangProjectile_001>();
                        boomerangProjectileObject.SetData(this.ItemData);
                        boomerangProjectileObject.Throw(player, (BoomerangData)this.ItemData);
                    }
                    break;
                case 2:
                    boomerangProjectileObject = BoomerangProjectileSpawner.Instance.Pool.Get().GetComponent<BoomerangProjectile_001>();
                    //boomerangProjectileObject = Instantiate(boomerangProjectilePrefab, transform.position, Quaternion.identity).GetComponent<BoomerangProjectile_001>();
                    boomerangProjectileObject.SetData(this.ItemData);
                    boomerangProjectileObject.Throw(player, (BoomerangData)this.ItemData);
                    break;
                default:
                    break;
            }
            


            return true;
        }


        private void ResetBoomerangState()
        {
            isReturning = true;
        }


        public int GetDamage()
        {
            return ((BoomerangData)ItemData).damage;
        }
    }
}

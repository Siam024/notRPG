using System;
namespace notRPG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Static method for displaying the welcome message
            ShowWelcomeMessage();

            // Player selection
            Console.WriteLine("\nChoose your character:\n1. Warrior   2. Mage\n");
            Player player = null;
            int choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    player = new Warrior("Sally the warrior");
                    break;
                case 2:
                    player = new Mage("Neo the Mage");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Exiting game.");
                    return;
            }

            // Game introduction
            Console.WriteLine($"You selected {player.name}. Let the adventure begin!");
            player.ShowStats();

            // Add items to inventory
            Items healthPotion = new Items("Health Potion", "Restores Health", 50);
            Items energyPotion = new Items("Energy Potion", "Restores Energy", 30);
            Items attackBoost = new Items("Attack Boost", "Increases Attack Power Temporarily", 10);
            
            Console.WriteLine("A villager gifted you an item for saving him from bandits");
            player.Inventory.AddItem(healthPotion);

            
            // Side quest
            NPC npc = new NPC("Defeat 3 minions to save the village ", 50);
            npc.OfferQuest(player);

            // First minion encounter
            Minions m1 = new Minions("Goblin", 20);
            Console.WriteLine("\n\nA Goblin appears! Do you want to attack?\n1. Yes 2. No\n");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    player.Attack(m1);
                    break;
                case 2:
                    Console.WriteLine("You let the Goblin go.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Goblin flees.");
                    break;
            }

            // Offer to fight another Goblin or proceed to Boss
            Console.WriteLine("\nDo you want to: \n1. Fight more Goblins for some hidden rewards    2. Proceed to the Boss\n");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.WriteLine("You decided to fight more minions.");
                    Minions m2 = new Minions("Goblin", 20);
                    Console.WriteLine("Another Goblin appears!");
                    player.Attack(m2);
                    Console.WriteLine("\nOne more Orc appears!");
                    Minions m3 = new Minions("Orc", 30);
                    player.Attack(m3); // After defeating 3, level up occurs
                    break;
                case 2:
                    Console.WriteLine("You decided to proceed to the Boss.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Proceeding to the Boss by default.");
                    break;
            }
            if (choice==1){
                Console.WriteLine("You got some rewards");
                // Adding items to the player's inventory
                
                player.Inventory.AddItem(energyPotion);
                player.Inventory.AddItem(attackBoost);

            }

            // Boss fight
            Boss boss = new Boss("Demon King", 200);
            Console.WriteLine($"The final boss {boss.name} appears !");

            while (boss.HP > 0 && player.HP > 0)
            {
            Console.WriteLine("Choose action: 1. Attack 2. Check Stats 3. Check Inventory 4. Use Item 5. Sell Item\n");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
                {
                    case 1:
                        if (player.energy > 0)
                        {
                            player.Attack(boss);
                            //boss.TakeDamage(30);
                            player.TakeDamage(15);
                            player.energy -= 10;
                        }
                        else
                        {
                            Console.WriteLine("Not enough energy to attack!");
                        }
                        break;
                    case 2:
                        player.ShowStats();
                        break;
                    case 3:
                        player.Inventory.ShowInventory();
                        break;
                    case 4:
                        UseItem(player);
                        break;
                    case 5:
                        player.Inventory.ShowInventory();
                        Console.WriteLine("Enter number which you want to sell");
                        int itemChoice = Convert.ToInt32(Console.ReadLine()) - 1;
                        player.Inventory.RemoveItem(itemChoice);
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
        }

            if (player.HP <= 0)
            {
                Console.WriteLine("\n\nYou have been defeated! Game Over.");
            }
            else
            {
                Console.WriteLine("\n\nCongratulations, you defeated the Boss!And saved the world");
            }
        }

        // Static method to display the welcome message
        static void ShowWelcomeMessage()
        {
            Console.WriteLine("\n\nWelcome to the RPG Game!");
        }
        static void UseItem(Player player)
        {
            Console.WriteLine("Enter the number of the item to use:");
            player.Inventory.ShowInventory();

            int itemChoice = Convert.ToInt32(Console.ReadLine()) - 1;
            if (itemChoice >= 0 && itemChoice < player.Inventory.capacity && player.Inventory.items[itemChoice] != null)
            {
                Items selectedItem = player.Inventory.items[itemChoice];
                selectedItem.UseItem();
                ApplyItemEffect(player, selectedItem);
                player.Inventory.RemoveItem(itemChoice);
            }
            else
            {
                Console.WriteLine("Invalid item choice.");
            }
        }
    
        static void ApplyItemEffect(Player player, Items item)
        {
        switch (item.effect)
        {
            case "Restores Health":
                player.HP += item.effectValue;
                Console.WriteLine($"{player.name} restored {item.effectValue} HP! Current HP: {player.HP}");
                break;
            case "Restores Energy":
                player.energy += item.effectValue;
                Console.WriteLine($"{player.name} restored {item.effectValue} energy! Current Energy: {player.energy}");
                break;
            case "Increases Attack Power Temporarily":
                Console.WriteLine($"{player.name}'s attack power increased by {item.effectValue} temporarily!");
                // You can modify player.attack temporarily if such a field exists.
                break;
            default:
                Console.WriteLine("Unknown item effect.");
                break;
        }
        }



        interface isDamagable
        {
            void TakeDamage(int damage);
        }

        abstract class Player : isDamagable
        {
            public string name;
            public int level;
            public int Exp;
            public int HP;
            public int energy = 100;
            public Inventory Inventory;
            private int minionsDefeated = 0; // Tracks defeated minions

            public Player(string name, int level, int Exp, int HP)
            {
                this.name = name;
                this.level = level;
                this.Exp = Exp;
                this.HP = HP;
                Inventory = new Inventory();
            }

            public abstract void LevelUp();
            public abstract void Attack(Enemy enemy);
            public abstract void ShowStats();

            public void TakeDamage(int damage)
            {
                HP -= damage;
                Console.WriteLine($"{name} took {damage} damage. HP left: {HP}");
                if (HP < 0) {
                    HP=0;
                }
                if (HP == 0){ 
                    Die();
                }
            }

            public void Die()
            {
                Console.WriteLine($"{name} has died.");
            }

            // Track defeated minions and evolve accordingly
            public void DefeatMinion()
            {
                minionsDefeated++;
                Exp += 34; // Reward for defeating a minion
                Console.WriteLine($"\n{name} defeated a minion! Total minions defeated: {minionsDefeated}");

                if (minionsDefeated == 3)
                {
                    LevelUp();
                    minionsDefeated = 0; // Reset count after level-up
                }
            }
        }

        class Warrior : Player
        {
            public Warrior(string name) : base(name, 1, 0, 110)
            {
            }

            public override void Attack(Enemy enemy)
            {
                int damage = 30;
                Console.WriteLine($"{name} attacks {enemy.name} for {damage} damage!");
                enemy.TakeDamage(damage);

                if (enemy.HP <= 0)
                {
                    if (enemy is Minions)
                    {
                        DefeatMinion(); // Track minion defeat
                    }
                }
            }

            public override void LevelUp()
            {
                level++;
                HP += 20;
                Console.WriteLine($"{name} leveled up to {level}! HP increased to {HP}.And gained some items");
                
            }

            public override void ShowStats()
            {
                Console.WriteLine($"\n\n{name} is a level {level} Warrior with {Exp} XP and {HP} HP.\n\n");
            }
        }

        class Mage : Player
        {
            public Mage(string name) : base(name, 1, 0, 90)
            {
            }

            public override void Attack(Enemy enemy)
            {
                int damage = 25;
                Console.WriteLine($"{name} casts a spell on {enemy.name} for {damage} damage!");
                enemy.TakeDamage(damage);

                if (enemy.HP <= 0)
                {
                    if (enemy is Minions)
                    {
                        DefeatMinion(); // Track minion defeat
                    }
                }
            }

            public override void LevelUp()
            {
                level++;
                HP += 15;
                Console.WriteLine($"{name} leveled up to {level}! HP increased to {HP}.");
            }

            public override void ShowStats()
            {
                Console.WriteLine($"{name} is a level {level} Mage with {Exp} XP and {HP} HP.\n\n");
            }
        }

        class NPC
        {
            public string Quest;
            public int Reward;

            public NPC(string quest, int reward)
            {
                Quest = quest;
                Reward = reward;
            }

            public void OfferQuest(Player player)
            {
                Console.WriteLine($"\n\nNPC: {Quest}. Reward: {Reward} XP.");
                Console.WriteLine($"{player.name} accepted the quest and earned {Reward} XP!");
                player.Exp += Reward;
            }
        }

        abstract class Enemy : isDamagable
        {
            public string name;
            public int HP;

            public Enemy(string name, int HP)
            {
                this.name = name;
                this.HP = HP;
            }

            public void Die()
            {
                Console.WriteLine($"{name} has died.");
            }

            public virtual void TakeDamage(int damage)
            {
                 HP -= damage;
                Console.WriteLine($"{name} took {damage} damage. HP left: {HP}");
                if (HP < 0) {
                    HP=0;
                }
                if (HP == 0){ 
                    Die();
                }
            }
        }

        class Boss : Enemy
        {
            public Boss(string name, int hp) : base(name, hp)
            {
            }

        public void TakeDamage(int damage, string v)
            {
                HP -= damage;
                Console.WriteLine($"{name} took {damage} {v} damage! Remaining HP: {HP}");
            }
        }

        class Minions : Enemy
        {
            public Minions(string name, int HP) : base(name, HP){}
        }

        class Items
        {
            public string name;
            public string effect;
            public int effectValue;

            public Items(string name, string effect, int effectValue)
            {
                this.name = name;
                this.effect = effect;
                this.effectValue = effectValue;
            }

            public void UseItem()
            {
                Console.WriteLine($"Using {name} with effect {effect} for {effectValue} value");
            }
        }

        class Inventory
        {
            public int capacity = 5;
            public Items[] items;
    
            public Inventory()
            {
                items = new Items[capacity];
            }
    
            public void AddItem(Items item)
            {
                for (int i = 0; i < capacity; i++)
                {
                    if (items[i] == null)
                    {
                        items[i] = item;
                        Console.WriteLine($"{item.name} added to inventory.");
                        return;
                    }
                }
                Console.WriteLine("Inventory is full. Cannot add new item.");
            }
    
            public void RemoveItem(int index)
            {
                if (index >= 0 && index < capacity && items[index] != null)
                {
                    Console.WriteLine($"{items[index].name} removed from inventory.");
                    items[index] = null;
                }
                else
                {
                    Console.WriteLine("Invalid index or item does not exist.");
                }
            }
    
            public void ShowInventory()
            {
                Console.WriteLine("Inventory:");
                for (int i = 0; i < capacity; i++)
                {
                    if (items[i] != null)
                    {
                        Console.WriteLine($"{i + 1}. {items[i].name} - {items[i].effect} ({items[i].effectValue})");
                    }
                    else
                    {
                        Console.WriteLine($"{i + 1}. Empty Slot");
                    }
                }
            }
        }
    }
}

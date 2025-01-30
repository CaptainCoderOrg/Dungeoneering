namespace CaptainCoder.DungeonCrawler.Combat;

public record EnemyCard
{
    public string Name { get; init; } = "Unnamed Monster";
    public int Speed { get; init; } = 3;
    public int MaxHealth { get; init; } = 2;
    public int Armor { get; init; } = 0;
    public IAttackRoll AttackRoll { get; init; } = new SimpleAttack(1, 2);
    public int Level { get; init; } = 1;
}
public record Enemy
{
    public static int NextId { get; private set; } = 0;
    public int Id { get; init; } = NextId++;
    public EnemyCard Card { get; init; } = new EnemyCard();
    public int Wounds { get; init; } = 0;
    public int Health => Card.MaxHealth - Wounds;
    public bool IsDead => Health <= 0;

}

public interface IEnemyCards
{
    public EnemyCard WeakSkeletonCard { get; }
    public EnemyCard SkeletonCard { get; }
    public EnemyCard EmployeeCard { get; }
    public EnemyCard HangryEmployee { get; }
    public EnemyCard MouthBreather { get; }
    public EnemyCard EyeKeyUh { get; }
    public EnemyCard Meatball { get; }
    public EnemyCard Wardrobe { get; }
    public EnemyCard CosmicBed { get; }
    public EnemyCard Karen { get; }
    public EnemyCard Chad { get; }
    public EnemyCard BloodShot { get; }
}

public class EasyEnemies : IEnemyCards
{
    public static IEnemyCards Enemies = new EasyEnemies();
    public EnemyCard WeakSkeletonCard { get; } = new()
    {
        Name = "Weak Skeleton",
        Speed = 5,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(1, 2)
    };
    public EnemyCard SkeletonCard { get; } = new()
    {
        Name = "Skeleton",
        Speed = 5,
        MaxHealth = 2,
        AttackRoll = new SimpleAttack(1, 2)
    };

    public EnemyCard EmployeeCard { get; } = new()
    {
        Name = "Disgruntled Employee",
        Speed = 2,
        Armor = 1,
        MaxHealth = 3,
        AttackRoll = new SimpleAttack(1, 3),
        Level = 2,
    };

    public EnemyCard HangryEmployee { get; } = new()
    {
        Name = "Hangry Employee",
        Speed = 2,
        Armor = 0,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(1, 5),
        Level = 2,
    };

    public EnemyCard MouthBreather { get; } = new()
    {
        Name = "Mouth Breather",
        Speed = 2,
        Armor = 2,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(2, 6),
        Level = 2,
    };

    public EnemyCard EyeKeyUh { get; } = new()
    {
        Name = "Eye Key Uh",
        Speed = 2,
        Armor = 8,
        MaxHealth = 60,
        AttackRoll = new SimpleAttack(1, 10),
        Level = 5,
    };

    public EnemyCard Meatball { get; } = new()
    {
        Name = "Cosmic Meatball",
        Speed = 6,
        Armor = 0,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(1, 6),
        Level = 2,
    };

    public EnemyCard Wardrobe { get; } = new()
    {
        Name = "Cosmic Wardrobe",
        Speed = 2,
        Armor = 2,
        MaxHealth = 4,
        AttackRoll = new SimpleAttack(5, 5),
        Level = 3,
    };

    public EnemyCard CosmicBed { get; } = new()
    {
        Name = "Cosmic Bed",
        Speed = 1,
        Armor = 3,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(6, 8),
        Level = 4,
    };

    public EnemyCard Karen { get; } = new()
    {
        Name = "Karen",
        Speed = 6,
        Armor = 0,
        MaxHealth = 10,
        AttackRoll = new SimpleAttack(6, 6),
        Level = 4,
    };

    public EnemyCard Chad { get; } = new()
    {
        Name = "Chad",
        Speed = 8,
        Armor = 6,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(5, 5),
        Level = 4,
    };

    public EnemyCard BloodShot { get; } = new()
    {
        Name = "Bloodshot Meatball",
        Speed = 2,
        Armor = 0,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(8, 8),
        Level = 4,
    };
}

public class NormalEnemies : IEnemyCards
{
    public static IEnemyCards Enemies = new NormalEnemies();
    public EnemyCard WeakSkeletonCard { get; } = new()
    {
        Name = "Weak Skeleton",
        Speed = 5,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(1, 1)
    };
    public EnemyCard SkeletonCard { get; } = new()
    {
        Name = "Skeleton",
        Speed = 5,
        MaxHealth = 2,
        AttackRoll = new SimpleAttack(1, 3)
    };

    public EnemyCard EmployeeCard { get; } = new()
    {
        Name = "Disgruntled Employee",
        Speed = 2,
        Armor = 1,
        MaxHealth = 3,
        AttackRoll = new SimpleAttack(1, 3),
        Level = 2,
    };

    public EnemyCard HangryEmployee { get; } = new()
    {
        Name = "Hangry Employee",
        Speed = 4,
        Armor = 0,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(1, 4),
        Level = 2,
    };

    public EnemyCard MouthBreather { get; } = new()
    {
        Name = "Mouth Breather",
        Speed = 2,
        Armor = 2,
        MaxHealth = 3,
        AttackRoll = new SimpleAttack(2, 6),
        Level = 2,
    };

    public EnemyCard EyeKeyUh { get; } = new()
    {
        Name = "Eye Key Uh",
        Speed = 2,
        Armor = 10,
        MaxHealth = 100,
        AttackRoll = new SimpleAttack(1, 15),
        Level = 5,
    };

    public EnemyCard Meatball { get; } = new()
    {
        Name = "Cosmic Meatball",
        Speed = 6,
        Armor = 0,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(1, 10),
        Level = 2,
    };

    public EnemyCard Wardrobe { get; } = new()
    {
        Name = "Cosmic Wardrobe",
        Speed = 2,
        Armor = 3,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(6, 6),
        Level = 3,
    };

    public EnemyCard CosmicBed { get; } = new()
    {
        Name = "Cosmic Bed",
        Speed = 1,
        Armor = 4,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(8, 8),
        Level = 4,
    };

    public EnemyCard Karen { get; } = new()
    {
        Name = "Karen",
        Speed = 6,
        Armor = 0,
        MaxHealth = 16,
        AttackRoll = new SimpleAttack(6, 8),
        Level = 4,
    };

    public EnemyCard Chad { get; } = new()
    {
        Name = "Chad",
        Speed = 8,
        Armor = 6,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(4, 10),
        Level = 4,
    };

    public EnemyCard BloodShot { get; } = new()
    {
        Name = "Bloodshot Meatball",
        Speed = 2,
        Armor = 0,
        MaxHealth = 10,
        AttackRoll = new SimpleAttack(10, 10),
        Level = 4,
    };
}

public class HardEnemies : IEnemyCards
{
    public static IEnemyCards Enemies = new HardEnemies();
    public EnemyCard WeakSkeletonCard { get; } = new()
    {
        Name = "Weak Skeleton",
        Speed = 5,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(1, 1)
    };
    public EnemyCard SkeletonCard { get; } = new()
    {
        Name = "Skeleton",
        Speed = 5,
        MaxHealth = 3,
        AttackRoll = new SimpleAttack(1, 3)
    };

    public EnemyCard EmployeeCard { get; } = new()
    {
        Name = "Disgruntled Employee",
        Speed = 2,
        Armor = 2,
        MaxHealth = 5,
        AttackRoll = new SimpleAttack(1, 5),
        Level = 2,
    };

    public EnemyCard HangryEmployee { get; } = new()
    {
        Name = "Hangry Employee",
        Speed = 4,
        Armor = 0,
        MaxHealth = 8,
        AttackRoll = new SimpleAttack(1, 8),
        Level = 2,
    };

    public EnemyCard MouthBreather { get; } = new()
    {
        Name = "Mouth Breather",
        Speed = 2,
        Armor = 2,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(2, 6),
        Level = 2,
    };

    public EnemyCard EyeKeyUh { get; } = new()
    {
        Name = "Eye Key Uh",
        Speed = 3,
        Armor = 12,
        MaxHealth = 150,
        AttackRoll = new SimpleAttack(5, 20),
        Level = 5,
    };

    public EnemyCard Meatball { get; } = new()
    {
        Name = "Cosmic Meatball",
        Speed = 6,
        Armor = 0,
        MaxHealth = 1,
        AttackRoll = new SimpleAttack(1, 10),
        Level = 2,
    };

    public EnemyCard Wardrobe { get; } = new()
    {
        Name = "Cosmic Wardrobe",
        Speed = 2,
        Armor = 4,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(6, 6),
        Level = 3,
    };

    public EnemyCard CosmicBed { get; } = new()
    {
        Name = "Cosmic Bed",
        Speed = 2,
        Armor = 4,
        MaxHealth = 6,
        AttackRoll = new SimpleAttack(8, 10),
        Level = 4,
    };

    public EnemyCard Karen { get; } = new()
    {
        Name = "Karen",
        Speed = 6,
        Armor = 0,
        MaxHealth = 16,
        AttackRoll = new SimpleAttack(6, 12),
        Level = 4,
    };

    public EnemyCard Chad { get; } = new()
    {
        Name = "Chad",
        Speed = 8,
        Armor = 6,
        MaxHealth = 4,
        AttackRoll = new SimpleAttack(4, 10),
        Level = 4,
    };

    public EnemyCard BloodShot { get; } = new()
    {
        Name = "Bloodshot Meatball",
        Speed = 2,
        Armor = 2,
        MaxHealth = 10,
        AttackRoll = new SimpleAttack(10, 10),
        Level = 4,
    };
}
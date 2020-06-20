using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ZenithGun.Items.Weapons.ZenithGun
{

	public class ZenithGunMinionBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("The Arsenal");
			Description.SetDefault("This is getting a little ridiculous.");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ProjectileType<SDMGMinion>()] == 0)
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class ZenithGun : ModItem
	{
		List<int> typeList = new List<int>();
		List<int> id = new List<int>();
		Random random = new Random();

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("The amalgamation of all your efforts.\nSummons the entire arsenal to fight for you.\n66% chance to not consume ammo");
		}

		public override void SetDefaults() {
			item.damage = 100;
			item.ranged = true;
			item.width = 100;
			item.height = 40;
			item.useTime = 4;
			item.useAnimation = 4;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.noMelee = true; //so the item's animation doesn't do damage
			item.knockBack = 4;
			item.value = 100000;
			item.rare = ItemRarityID.Red;
			item.UseSound = SoundID.Item11;
			item.autoReuse = true;
			item.shoot = 10; //idk why but all the guns in the vanilla source have this
			item.shootSpeed = 16f;
			item.useAmmo = AmmoID.Bullet;
			item.buffType = BuffType<ZenithGunMinionBuff>();
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddIngredient(ItemID.SDMG, 1);
			recipe.AddIngredient(ItemID.VortexBeater, 1);
			recipe.AddIngredient(ItemID.Xenopopper, 1);
			recipe.AddIngredient(ItemID.ChainGun, 1);
			recipe.AddIngredient(ItemID.VenusMagnum, 1);
			recipe.AddIngredient(ItemID.Megashark, 1);
			recipe.AddIngredient(ItemID.OnyxBlaster, 1);
			recipe.AddIngredient(ItemID.ClockworkAssaultRifle, 1);
			recipe.AddIngredient(ItemID.PhoenixBlaster, 1);
			ModRecipe alternateRecipe = recipe;
			recipe.AddIngredient(ItemID.TheUndertaker, 1);
			alternateRecipe.AddIngredient(ItemID.Musket, 1);
			recipe.AddRecipe();
			alternateRecipe.AddRecipe();

		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			player.AddBuff(item.buffType, 10);
			Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(5));
			speedX = perturbedSpeed.X;
			speedY = perturbedSpeed.Y;
			int gunCount = player.ownedProjectileCounts[ProjectileType<SDMGMinion>()];
			if (gunCount == 0)
            {
				id.Clear();
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<SDMGMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<ChainGunMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<ClockworkAssaultRifleMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<MegasharkMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<VenusMagnumMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<OnyxBlasterMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<PhoenixBlasterMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<XenopopperMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				if(random.Next(2) == 0)
				{
					id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<MusketMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				}
				else
				{
					id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<TheUndertakerMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));
				}
				id.Add(Projectile.NewProjectile(position.X, position.Y, 0, 0, ProjectileType<VortexBeaterMinion>(), damage, knockBack, Main.myPlayer, 0f, 0f));

				typeList.Clear();
				typeList.Add(ProjectileID.CrystalBullet);
				typeList.Add(ProjectileID.CursedBullet);
				typeList.Add(ProjectileID.ChlorophyteBullet);
				typeList.Add(ProjectileID.BulletHighVelocity);
				typeList.Add(ProjectileID.IchorBullet);
				typeList.Add(ProjectileID.VenomBullet);
				typeList.Add(ProjectileID.PartyBullet);
				typeList.Add(ProjectileID.NanoBullet);
				typeList.Add(ProjectileID.Bullet);
				typeList.Add(ProjectileID.GoldenBullet);
			} 
			if (type == ProjectileID.Bullet)
			{
				//fire a random bullet from each gun
				for (int j = typeList.Count - 1; j > 1; j--)
				{
					int rnd = random.Next(j + 1);

					int value = typeList[rnd];
					typeList[rnd] = typeList[j];
					typeList[j] = value;
				}
				for (int i = 0; i < id.Count; i++)
                {
					Vector2 projectilePosition = Main.projectile[id[i]].Center;
					Vector2 mouseDirection = Main.MouseWorld - projectilePosition;
					mouseDirection = mouseDirection.RotatedByRandom(MathHelper.ToRadians(5));
					Projectile.NewProjectile(projectilePosition.X, projectilePosition.Y, mouseDirection.X, mouseDirection.Y, typeList[i], damage, knockBack, Main.myPlayer, 0f, 0f);
				}
			}
            else
            {
				for (int i = 0; i < id.Count; i++)
				{
					Vector2 projectilePosition = Main.projectile[id[i]].Center;
					Vector2 mouseDirection = Main.MouseWorld - projectilePosition;
					mouseDirection = mouseDirection.RotatedByRandom(MathHelper.ToRadians(5));
					Projectile.NewProjectile(projectilePosition.X, projectilePosition.Y, mouseDirection.X, mouseDirection.Y, type, damage, knockBack, Main.myPlayer, 0f, 0f);
				}
			}
			return true;
		}

		public override bool ConsumeAmmo(Player player)
		{
			return Main.rand.NextFloat() >= .66f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-14, 0);
		}
	}

	public class SDMGMinion : ModProjectile
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Animated Gun");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 1;
			// This is necessary for right-click targeting
			//ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width = 66;
			projectile.height = 32;
			// Makes the minion go through tiles freely
			projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			projectile.friendly = true;
			// Only determines the damage type
			projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			projectile.minionSlots = 0f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage()
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(BuffType<ZenithGunMinionBuff>());
			}
			if (player.HasBuff(BuffType<ZenithGunMinionBuff>()))
			{
				projectile.timeLeft = 2;
			}
			#endregion

			#region General behavior
			Vector2 idlePosition = player.Center;
			double theta = ((projectile.minionPos - 1)*-Math.PI/9d);
			idlePosition.X += 64f*(float)Math.Cos(theta) - 32f;
			idlePosition.Y += 64f*(float)Math.Sin(theta);
			idlePosition.Y -= 10f;

			// Teleport to player if distance is too big
			Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();
			if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f)
			{
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				projectile.position = idlePosition;
				projectile.velocity *= 0.1f;
				projectile.netUpdate = true;
			}

			//projectile.position = idlePosition;
			projectile.position += (idlePosition - projectile.position) * new Vector2(0.2f, 0.2f);
			#endregion

			#region Animation and visuals

			Vector2 mouseDirection = Main.MouseWorld - projectile.Center;
			float projectileAngle = mouseDirection.ToRotation()%(float)(2*Math.PI);
			if(projectileAngle < (float)(Math.PI/2f) && projectileAngle > (float)(-Math.PI/2f))
			{
				projectile.rotation = projectileAngle;
				projectile.spriteDirection = 1;
			} else
            {
				projectile.rotation = projectileAngle + (float)Math.PI;
				projectile.spriteDirection = -1;
            }
			

			// Some visuals here
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion
		}
	}

	public class ChainGunMinion : SDMGMinion
    {
		public sealed override void SetDefaults()
		{
			projectile.width = 52;
			projectile.height = 32;
			projectile.minionPos = 2;
		}
	}

	public class ClockworkAssaultRifleMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 56;
			projectile.height = 20;
			projectile.minionPos = 3;
		}
	}

	public class MegasharkMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 70;
			projectile.height = 28;
			projectile.minionPos = 4;
		}
	}

	public class VenusMagnumMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 52;
			projectile.height = 28;
			projectile.minionPos = 5;
		}
	}

	public class OnyxBlasterMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 60;
			projectile.height = 26;
			projectile.minionPos = 6;
		}
	}

	public class PhoenixBlasterMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 42;
			projectile.height = 30;
			projectile.minionPos = 7;
		}
	}

	public class XenopopperMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 42;
			projectile.height = 22;
			projectile.minionPos = 8;
		}
	}

	public class TheUndertakerMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 46;
			projectile.height = 36;
			projectile.minionPos = 9;
		}
	}

	public class MusketMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 56;
			projectile.height = 18;
			projectile.minionPos = 9;
		}
	}

	public class VortexBeaterMinion : SDMGMinion
	{
		public sealed override void SetDefaults()
		{
			projectile.width = 66;
			projectile.height = 28;
			projectile.minionPos = 10;
		}
	}

}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ZenithGun.Items.Weapons.ZenithGun {
	public class ZenithGunMinionBuff : ModBuff {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("The Arsenal");
			Description.SetDefault("This is getting a little ridiculous.");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			if (player.ownedProjectileCounts[ProjectileType<SDMGMinion>()] == 0) {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class ZenithGun : ModItem {
		List<int> typeList = new List<int>();
		List<int> id = new List<int>();
		Random random = new Random();

		public override void SetStaticDefaults() {
			Tooltip.SetDefault("The amalgamation of all your efforts.\nSummons the entire arsenal to fight for you.\n66% chance to not consume ammo");
		}

		public override void SetDefaults() {
			Item.damage = 100;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 100;
			Item.height = 40;
			Item.useTime = 4;
			Item.useAnimation = 4;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true; //so the item's animation doesn't do damage
			Item.knockBack = 4;
			Item.value = 100000;
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
			Item.shoot = 10; //idk why but all the guns in the vanilla source have this
			Item.shootSpeed = 16f;
			Item.useAmmo = AmmoID.Bullet;
			Item.buffType = BuffType<ZenithGunMinionBuff>();
		}

		public override void AddRecipes() {
			Recipe baseRecipe = CreateRecipe()
				.AddTile(TileID.MythrilAnvil)
				.AddIngredient(ItemID.SDMG, 1)
				.AddIngredient(ItemID.VortexBeater, 1)
				.AddIngredient(ItemID.Xenopopper, 1)
				.AddIngredient(ItemID.ChainGun, 1)
				.AddIngredient(ItemID.VenusMagnum, 1)
				.AddIngredient(ItemID.Megashark, 1)
				.AddIngredient(ItemID.OnyxBlaster, 1)
				.AddIngredient(ItemID.ClockworkAssaultRifle, 1)
				.AddIngredient(ItemID.PhoenixBlaster, 1);

			Recipe cloneRecipe = baseRecipe.Clone();

			ModLoader.TryGetMod("CalamityMod", out Mod calamityMod);
			if (calamityMod != null && calamityMod.TryFind<ModItem>("AuricBar", out ModItem AuricBar)) {
				baseRecipe.AddIngredient(ItemID.TheUndertaker, 1)
					.AddIngredient(AuricBar.Type, 5)
					.Register();
				cloneRecipe.AddIngredient(ItemID.Musket, 1)
					.AddIngredient(AuricBar.Type, 5)
					.Register();
			} else {
				baseRecipe.AddIngredient(ItemID.TheUndertaker, 1)
					.Register();
				cloneRecipe.AddIngredient(ItemID.Musket, 1)
					.Register();
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			player.AddBuff(Item.buffType, 10);
			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));

			int gunCount = player.ownedProjectileCounts[ProjectileType<SDMGMinion>()];
			if (gunCount == 0) {
				id.Clear();
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<SDMGMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<ChainGunMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<ClockworkAssaultRifleMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<MegasharkMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<VenusMagnumMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<OnyxBlasterMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<PhoenixBlasterMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<XenopopperMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));

				if (random.Next(2) == 0) {
					id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<MusketMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				} else {
					id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<TheUndertakerMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));
				}

				id.Add(Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, new Vector2(0, 0), ProjectileType<VortexBeaterMinion>(), damage, knockback, Main.myPlayer, 0f, 0f));

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

			if (type == ProjectileID.Bullet) {
				//fire a random bullet from each gun
				for (int j = typeList.Count - 1; j > 1; j--) {
					int rnd = random.Next(j + 1);

					int value = typeList[rnd];
					typeList[rnd] = typeList[j];
					typeList[j] = value;
				}

				for (int i = 0; i < id.Count; i++) {
					Vector2 projectilePosition = Main.projectile[id[i]].Center;
					Vector2 mouseDirection = Main.MouseWorld - projectilePosition;
					mouseDirection = mouseDirection.RotatedByRandom(MathHelper.ToRadians(5));
					Projectile.NewProjectile(Main.projectile[id[i]].GetSource_FromThis(), projectilePosition, mouseDirection, typeList[i], damage, knockback, Main.myPlayer, 0f, 0f);
				}
			} else {
				for (int i = 0; i < id.Count; i++) {
					Vector2 projectilePosition = Main.projectile[id[i]].Center;
					Vector2 mouseDirection = Main.MouseWorld - projectilePosition;
					mouseDirection = mouseDirection.RotatedByRandom(MathHelper.ToRadians(5));
					Projectile.NewProjectile(Main.projectile[id[i]].GetSource_FromThis(), projectilePosition, mouseDirection, type, damage, knockback, Main.myPlayer, 0f, 0f);
				}
			}

			return true;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player) {
			return Main.rand.NextFloat() >= .66f;
		}

		public override Vector2? HoldoutOffset() {
			return new Vector2(-14, 0);
		}
	}

	public class SDMGMinion : ModProjectile {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Animated Gun");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;
			// This is necessary for right-click targeting
			//ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 66;
			Projectile.height = 32;
			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;
			// Only determines the damage type
			Projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 0f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return false;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active) {
				player.ClearBuff(BuffType<ZenithGunMinionBuff>());
			}

			if (player.HasBuff(BuffType<ZenithGunMinionBuff>())) {
				Projectile.timeLeft = 2;
			}
			#endregion

			#region General behavior
			Vector2 idlePosition = player.Center;
			double theta = ((Projectile.minionPos - 1)*-Math.PI/9d);
			idlePosition.X += 64f * (float) Math.Cos(theta) - 32f;
			idlePosition.Y += 64f * (float) Math.Sin(theta);
			idlePosition.Y -= 10f;

			// Teleport to player if distance is too big
			Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();
			if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f) {
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			//projectile.position = idlePosition;
			Projectile.position += (idlePosition - Projectile.position) * new Vector2(0.2f, 0.2f);
			#endregion

			#region Animation and visuals

			Vector2 mouseDirection = Main.MouseWorld - Projectile.Center;
			float projectileAngle = mouseDirection.ToRotation()%(float)(2*Math.PI);
			if (projectileAngle < (float) (Math.PI / 2f) && projectileAngle > (float) (-Math.PI / 2f)) {
				Projectile.rotation = projectileAngle;
				Projectile.spriteDirection = 1;
			} else {
				Projectile.rotation = projectileAngle + (float) Math.PI;
				Projectile.spriteDirection = -1;
			}


			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion
		}
	}

	public class ChainGunMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 52;
			Projectile.height = 32;
			Projectile.minionPos = 2;
		}
	}

	public class ClockworkAssaultRifleMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 56;
			Projectile.height = 20;
			Projectile.minionPos = 3;
		}
	}

	public class MegasharkMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 70;
			Projectile.height = 28;
			Projectile.minionPos = 4;
		}
	}

	public class VenusMagnumMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 52;
			Projectile.height = 28;
			Projectile.minionPos = 5;
		}
	}

	public class OnyxBlasterMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 60;
			Projectile.height = 26;
			Projectile.minionPos = 6;
		}
	}

	public class PhoenixBlasterMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 42;
			Projectile.height = 30;
			Projectile.minionPos = 7;
		}
	}

	public class XenopopperMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 42;
			Projectile.height = 22;
			Projectile.minionPos = 8;
		}
	}

	public class TheUndertakerMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 46;
			Projectile.height = 36;
			Projectile.minionPos = 9;
		}
	}

	public class MusketMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 56;
			Projectile.height = 18;
			Projectile.minionPos = 9;
		}
	}

	public class VortexBeaterMinion : SDMGMinion {
		public sealed override void SetDefaults() {
			Projectile.width = 66;
			Projectile.height = 28;
			Projectile.minionPos = 10;
		}
	}
}

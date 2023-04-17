using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;


namespace ZenithGun.Items.Weapons.ZenithStaff {
	public class ZenithStaffMinionBuff : ModBuff {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Zenith Staff Minion");
			Description.SetDefault("The example minion will fight for you");
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			if (player.ownedProjectileCounts[ProjectileType<ZenithStaffMinion>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			} else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class ZenithStaffItem : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Zenith Staff");
			Tooltip.SetDefault("Summons an example minion to fight for you\nUses 11 minion slots");
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
			ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
		}

		public override void SetDefaults() {
			Item.damage = 200;
			Item.knockBack = 6f;
			Item.mana = 20;
			Item.width = 52;
			Item.height = 52;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item44;

			// These below are needed for a minion weapon
			Item.DamageType = DamageClass.Summon;
			Item.buffType = BuffType<ZenithStaffMinionBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ProjectileType<ZenithStaffMinion>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			if (player.ownedProjectileCounts[ProjectileType<ZenithStaffMinion>()] == 0) {
				player.AddBuff(Item.buffType, 2);
			}

			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes() {
			Recipe baseRecipe = CreateRecipe()
				.AddTile(TileID.LunarCraftingStation)
				.AddIngredient(ItemID.DirtBlock, 1)
				.Register();
		}
	}

	/*
	 * This minion shows a few mandatory things that make it behave properly. 
	 * Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	 * If the player targets a certain NPC with right-click, it will fly through tiles to it
	 * If it isn't attacking, it will float near the player with minimal movement
	 */
	public class ZenithStaffMinion : ModProjectile {
		Random random;
		double theta = -1;
		bool isSpinning, isSlashing = false;
		Vector2 direction;

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Zenith Minion");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 1;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults() {
			Projectile.width = 94;
			Projectile.height = 30;
			// Makes the minion go through tiles freely
			Projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.friendly = true;
			// Only determines the damage type
			Projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.minionSlots = 1f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.penetrate = -1;

			random = new Random();
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		public float Timer {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active) {
				player.ClearBuff(BuffType<ZenithStaffMinionBuff>());
			}
			if (player.HasBuff(BuffType<ZenithStaffMinionBuff>())) {
				Projectile.timeLeft = 2;
			}
			#endregion

			#region General behavior
			Vector2 idlePosition = player.Center;
			idlePosition.Y -= 96f;
			idlePosition.X -= 48f;

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

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

			// If your minion is flying, you want to do this independently of any conditions
			/*
			float overlapVelocity = 0.04f;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				// Fix overlap with other minions
				Projectile other = Main.projectile[i];
				if (i != projectile.whoAmI && other.active && other.owner == projectile.owner && Math.Abs(projectile.position.X - other.position.X) + Math.Abs(projectile.position.Y - other.position.Y) < projectile.width)
				{
					if (projectile.position.X < other.position.X) projectile.velocity.X -= overlapVelocity;
					else projectile.velocity.X += overlapVelocity;

					if (projectile.position.Y < other.position.Y) projectile.velocity.Y -= overlapVelocity;
					else projectile.velocity.Y += overlapVelocity;
				}
			}
			*/
			#endregion

			#region Find target
			// Starting search distance
			float distanceFromTarget = 700f;
			Vector2 targetCenter = Projectile.position;
			Vector2 targetCenterOffset = targetCenter;
			bool foundTarget = false;

			// This code is required if your minion weapon has the targeting feature
			if (player.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);
				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					if (theta == -1)
						theta = random.NextDouble() * Math.PI * 2;
					targetCenterOffset = targetCenter;
					targetCenterOffset.X += 64f * (float) Math.Cos(theta);
					targetCenterOffset.Y += 64f * (float) Math.Sin(theta);
					foundTarget = true;
				}
			}
			if (!foundTarget) {
				// This code is required either way, used for finding a target
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						//bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						//bool closeThroughWall = between < 100f;
						if (((closest && inRange) || !foundTarget)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							if (theta == -1)
								theta = random.NextDouble() * Math.PI * 2;
							targetCenterOffset = targetCenter;
							targetCenterOffset.X += 200f * (float) Math.Cos(theta);
							targetCenterOffset.Y += 200f * (float) Math.Sin(theta);
							foundTarget = true;
						}
					}
				}
			}

			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
			#endregion

			#region Movement

			// Default movement parameters (here for attacking)
			float speed = 50f;
			float inertia = 10f;

			if (isSlashing) {
				Timer++;
				if (Timer > 20) {
					Projectile.velocity = Vector2.Zero;
					theta += Math.PI + random.NextDouble() - 0.5;
					isSlashing = false;
					isSpinning = false;
					Timer = 0;
				} else if (foundTarget) {
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
				}
			} else if (foundTarget) {
				Timer++;
				Projectile.position += (targetCenterOffset - Projectile.position) * new Vector2(0.05f, 0.05f);

				if (Timer > 90) {
					isSlashing = true;
					isSpinning = random.NextDouble() > 0.5;
					direction = targetCenter - Projectile.Center;
					direction.Normalize();
					direction *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
					Timer = 0;
				}

			} else {
				// Minion doesn't have a target: return to player and idle
				Projectile.position += (idlePosition - Projectile.position) * new Vector2(0.05f, 0.05f);
				Projectile.velocity = Vector2.Zero;
				isSpinning = false;
			}

			Projectile.friendly = isSlashing;
			#endregion

			#region Animation and visuals

			if (isSpinning) {
				Projectile.rotation += 1f;
			} else if (!isSlashing) {
				if (foundTarget) {
					Projectile.rotation = (targetCenter - Projectile.Center).ToRotation();
				} else {
					Projectile.rotation = (float) Math.PI / 2f;
				}
			}

			/*
			// This is a simple "loop through all frames from top to bottom" animation
			int frameSpeed = 5;
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
			*/

			// Some visuals here
			Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion
		}
	}
}

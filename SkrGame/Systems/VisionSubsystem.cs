﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEngine.Components;
using DEngine.Entities;
using SkrGame.Universe;
using SkrGame.Universe.Entities;
using SkrGame.Universe.Entities.Actors;

namespace SkrGame.Systems {
	public class VisionSubsystem {
		private FilteredCollection entities;		

		public VisionSubsystem(World world) {
			entities = world.EntityManager.Get(typeof(GameObject), typeof(SightComponent));			
		}

		public void Update() {
			foreach (var entity in entities) {
				var p = entity.Get<GameObject>();
				entity.Get<SightComponent>().CalculateSight();
			}
		}
	}
}

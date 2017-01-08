﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float speed = 10f;
	public float fireRate = 0.3f;
	public float health = 10;
	public int score = 100;
	public int showDamageForFrame = 2;
	public bool _________________________;

	public Bounds bounds;
	public Vector3 boundsCenterOffset;
	public Color[] originalColors;
	public Material[] materials;
	public int remainingDamageFrames = 0;

	void Awake(){
		materials = Utils.GetAllMaterials (gameObject);
		originalColors = new Color[materials.Length];
		for (int i = 0; i < materials.Length; i++) {
			originalColors [i] = materials [i].color;
		}
		InvokeRepeating ("CheckOffScreen", 0f, 2f);
	}

	void Update(){
		Move ();
		if (remainingDamageFrames > 0) {
			remainingDamageFrames--;
			if (remainingDamageFrames == 0) {
				UnShowDamage ();
			}
		}
	}

	public virtual void Move(){
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	public Vector3 pos{
		get{
			return(this.transform.position);
		}
		set{
			this.transform.position = value;
		}
	}

	void CheckOffScreen(){
		if (bounds.size == Vector3.zero) {
			bounds = Utils.CombineBoundsOfChildren (this.gameObject);
			boundsCenterOffset = bounds.center - transform.position;
		}

		bounds.center = transform.position + boundsCenterOffset;
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen);
		if (off != Vector3.zero) {
			if (off.y < 0) {
				Destroy (this.gameObject);
			}
		}
	}

	void OnCollisionEnter(Collision coll){
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent<Projectile> ();
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero ||
			    Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy (other);
				break;
			}

			ShowDamage ();
			health -= Main.W_DEFS [p.type].damageOnHit;
			if (health <= 0) {
				Destroy (this.gameObject);
			}
			Destroy (other);
			break;
		}
	}

	void ShowDamage(){
		foreach (Material m in materials) {
			m.color = Color.red;
		}

		remainingDamageFrames = showDamageForFrame;
	}

	void UnShowDamage(){
		for (int i = 0; i < materials.Length; i++) {
			materials [i].color = originalColors [i];
		}
	}
}
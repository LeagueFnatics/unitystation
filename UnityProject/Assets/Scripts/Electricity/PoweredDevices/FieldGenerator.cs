﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class FieldGenerator : InputTrigger , IElectricalNeedUpdate
{
	public PoweredDevice poweredDevice;

	[SyncVar(hook = "CheckState")]
	public bool isOn = false;
	public bool connectedToOther = false;
	private Coroutine coSpriteAnimator;

	public Sprite offSprite;
	public Sprite onSprite;
	public Sprite[] searchingSprites;
	public Sprite[] connectedSprites;

	public SpriteRenderer spriteRend;

	List<Sprite> animSprites = new List<Sprite>();

	public float Voltage;

	public float  Resistance = 240;
	public float  PreviousResistance = 240;
	public PowerTypeCategory ApplianceType = PowerTypeCategory.FieldGenerator;
	public HashSet<PowerTypeCategory> CanConnectTo = new HashSet<PowerTypeCategory>();

	public override void OnStartClient()
	{

		base.OnStartClient();
		poweredDevice.CanConnectTo = CanConnectTo;
		poweredDevice.Categorytype = ApplianceType;
		poweredDevice.PassedDownResistance = Resistance;
		poweredDevice.CanProvideResistance = true;
		CheckState(isOn);
		if (!(ElectricalSynchronisation.PoweredDevices.Contains(this))){
			ElectricalSynchronisation.PoweredDevices.Add (this);
		}
	}
		
	public override void Interact(GameObject originator, Vector3 position, string hand)
	{
		if (!isServer) {
			InteractMessage.Send(gameObject, hand);
		} else {
			isOn = !isOn;
			CheckState(isOn);
		}
	}

	public void PowerUpdateStructureChange(){
	}
	public void PowerUpdateStructureChangeReact(){
	}
	public void PowerUpdateResistanceChange(){
	}
	public void PowerUpdateCurrentChange (){
	}

	//Power supply updates
	void SupplyUpdate(){
		CheckState(isOn);
	}

	public void PowerNetworkUpdate(){
		if (Resistance != PreviousResistance) {
			poweredDevice.PassedDownResistance = Resistance;
			PreviousResistance = Resistance;
			ElectricalSynchronisation.ResistanceChange = true;
			ElectricalSynchronisation.CurrentChange = true;
		}
		Voltage = poweredDevice.ActualVoltage;
		//Logger.Log (Voltage.ToString ());
	}

	//Check the operational state
	void CheckState(bool _isOn){
		if(isOn){
			//				if(poweredDevice.suppliedElectricity.current == 0){
			//					if (coSpriteAnimator != null) {
			//						StopCoroutine(coSpriteAnimator);
			//						coSpriteAnimator = null;
			//					}
			//					spriteRend.sprite = onSprite;
			//				}
			//				if(poweredDevice.suppliedElectricity.current > 15){
			//					if(!connectedToOther){
			//						animSprites = new List<Sprite>(searchingSprites);
			//						if (coSpriteAnimator == null) {
			//							coSpriteAnimator = StartCoroutine(SpriteAnimator());
			//						}
			//					} else {
			//						animSprites = new List<Sprite>(connectedSprites);
			//						if(coSpriteAnimator == null) {
			//							coSpriteAnimator = StartCoroutine(SpriteAnimator());
			//						}
			//					}
			//				}
		} else {
			if (coSpriteAnimator != null) {
				StopCoroutine(coSpriteAnimator);
				coSpriteAnimator = null;
			}
			spriteRend.sprite = offSprite;
		}
	}

	IEnumerator SpriteAnimator(){
		int index = 0;
		while(true){
			Debug.Log("animating shield");
			if(index >= animSprites.Count){
				index = 0;
			}
			spriteRend.sprite = animSprites[index];
			index++;
			yield return new WaitForSeconds(0.3f);
		}
	}

	public void OnDestroy(){
		ElectricalSynchronisation.StructureChangeReact = true;
		ElectricalSynchronisation.ResistanceChange = true;
		ElectricalSynchronisation.CurrentChange = true;
		ElectricalSynchronisation.PoweredDevices.Remove(this);
		//Then you can destroy
	}
}


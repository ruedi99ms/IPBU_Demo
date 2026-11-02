// ****************************************************************
// Unity Game Engine C#-Script - First Person
// Everything in this file by MrLarodos: http://www.youtube.com/user/MrLarodos
//
// Released under the Creative Commons Attribution 3.0 Unported License:
// http://creativecommons.org/licenses/by/3.0/de/
// http://creativecommons.org/licenses/by/3.0/
//
// If you use this file or parts of it, you have to include this information header.
//
// DEUTSCH:
// Wenn Du diese Datei oder Teile davon benutzt, musst Du diesen Infoteil beilegen.
// ****************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fps_controller : MonoBehaviour{
	
	[Tooltip("Die Kamera bzw. der Kopf des Spielers")]
	public GameObject head_cam; //Die Kamera bzw. der Kopf des Spielers

	[Tooltip("Fortbewegung erlaubt (true) oder verboten (false)")]
	public bool move_allowed = true; //Fortbewegung erlaubt (true) oder verboten (false)

	[Tooltip("Normale Laufgeschwindigkeit")]
	public float walk_speed = 5.0F; //Normale Laufgeschwindigkeit

	[Tooltip("Beim Rennen wird walk_speed um diesen Faktor erhöht")]
	public float run_speed_factor = 1.5F; //Beim Rennen wird walk_speed um diesen Faktor erhöht

	[Tooltip("Springen erlaubt (true) oder verboten (false)")]
	public bool jump_allowed = true; //Springen erlaubt (true) oder verboten (false)

	[Tooltip("Sprungkraft des Spielers")]
	public float jump_force = 8.0F; //Sprungkraft des Spielers

	[Tooltip("Sprungverzögerung")]
	public float jump_delay = 1.0F; //Sprungverzögerung

	[Tooltip("Bodenkontaktstrahl nach unten, um festzustellen, ob Spieler in Bodenkontakt")]
	public float down_length = 1.1F; //Bodenkontaktstrahl nach unten, um festzustellen, ob Spieler in Bodenkontakt
	
	[Tooltip("Kopfbewegung erlaubt (true) oder verboten (false)")]
	public bool turn_allowed = true; //Kopfbewegung erlaubt (true) oder verboten (false)

	[Tooltip("Drehgeschwindigkeit der Kamera / des Kopfes")]
	public float cam_turn_speed = 2.0F; //Drehgeschwindigkeit der Kamera / des Kopfes

	[Tooltip("Maximaler Winkel / Rotationslimit der Kamera nach oben und unten")]
	public float cam_x_rot_limit = 55.0F; //Maximaler Winkel / Rotationslimit der Kamera nach oben und unten

	[Tooltip("Interaktion möglich (true) oder nicht möglich (false)")]
	public bool interaction_allowed = false; //Kopfbewegung erlaubt (true) oder verboten (false)

	[Tooltip("Reichweite der Hand des Spielers")]
	public float hand_range = 2.0F; //Reichweite der Hand des Spielers

	[Tooltip("Benutzen-Taste des Spielers")]
	public string use_key = "e"; //Benutzen-Taste des Spielers

	[Tooltip("Textobjekt der UI")]
	public Text hand_ui_texter; //Textobjekt der UI

	[Tooltip("Fadenkreuz im UI")]
	public Image crosshair_ui; //Fadenkreuz im UI

	[Tooltip("Standardbild des Fadenkreuzes")]
	public Sprite crosshair_standard; //Standardbild des Fadenkreuzes

	[Tooltip("Positiv-Variante des Fadenkreuzes")]
	public Sprite crosshair_positive; //Positiv-Variante des Fadenkreuzes

	[Tooltip("Negativ-Variante des Fadenkreuzes")]
	public Sprite crosshair_negative; //Negativ-Variante des Fadenkreuzes

	[Tooltip("Strecke der Auf- und Abbewegung des Kopfes beim Laufen")]
	public float head_hop_height = 0.25f; //Strecke der Auf- und Abbewegung des Kopfes beim Laufen

	[Tooltip("Zeit für die Auf- und Abbewegung des Kopfes beim Laufen")]
	public float head_hop_time = 0.075f; //Zeit für die Auf- und Abbewegung des Kopfes beim Laufen

	[Tooltip("Kriechen und Ducken zulässig")]
	public bool crouch_allowed = true; //Kriechen und Ducken zulässig

	[Tooltip("Beim Kriechen wird walk_speed um diesen Faktor verringert")]
	public float crouch_speed_factor = 0.75F; //Beim Kriechen wird walk_speed um diesen Faktor verringert

	[Tooltip("Geschwindigkeit, wie schnell die geduckte Haltung erreicht wird")]
	public float crouch_pos_speed = 5.0F; //Geschwindigkeit, wie schnell die geduckte Haltung erreicht wird

	[Tooltip("Wieviel wird die Höhe des Spielers beim Ducken reduziert")]
	public float crouch_pos_factor = 0.25F; //Wieviel wird die Höhe des Spielers beim Ducken reduziert

	[Tooltip("Schritt und Sprungssounds aktivieren")]
	public bool sounds_active; //Schritt und Sprungssounds aktivieren

	[Tooltip("Zufällige Schrittgeräuschauswahl")]
	public List<AudioClip> step_sounds; //Zufällige Schrittgeräuschauswahl

	[Tooltip("Sound beim Absprung vom Boden")]
	public AudioClip jump_step_sound; //Sound beim Absprung vom Boden

	[Tooltip("Sound beim Landen auf dem Boden")]
	public AudioClip jump_land_sound; //Sound beim Landen auf dem Boden

	float cam_x_rot_val = 0; //Winkel der aktuellen Kopfneigung (kleinerer Wert = höherer Blick)

	GameObject cam;
	AudioSource audio_source;
	Rigidbody my_rigid_body;

	RaycastHit down_ray;
	bool floor_touch;
	bool floor_touch_before;
	GameObject floor_ob;

	RaycastHit hand_ray;
	bool hand_touch;
	GameObject hand_ob;
	Vector3 cam_standard_pos;
	bool head_up;
	Vector3 original_size_v3;

	bool key_left;
	bool key_forward;
	bool key_backwards;
	bool key_right;
	bool key_run;
	bool key_crouch;
	bool key_jump;
	bool key_use;
	bool use_trigger;

	bool jump_trigger;
	bool jump_has_ended;
	float jump_timer;

	float head_2_target_lerp_val;
	float head_2_target_lerp_min;
	float head_2_target_lerp_max;
	bool lerp_reset; //Wird gesetzt, wenn der Spieler vom Laufen ins Rennen oder umgekehrt wechselt, um damit head_2_target_lerp_val zu resetten

	void Awake(){

		my_rigid_body = this.GetComponent<Rigidbody>();
		audio_source = GetComponent<AudioSource>();
		original_size_v3 = this.transform.localScale;

		if( !my_rigid_body ){
			print("Keine RigidBody im Spieler " + this.name + " gefunden! Spieler wird entfernt.");
			Destroy(this.gameObject);
			return;
		}

		try{
			if(!head_cam){ //Wenn in der Public-Variable head_cam nichts festgelegt wurde, Kamera mit Namen "head_cam" in children suchen
				cam = transform.Find("head_cam").gameObject;
				head_cam = cam;
			}else{
				cam = head_cam;
			}
		}catch{
			print("Kein 'head_cam'-Kopfkameraobjekt am Spieler '" + this.name + "' gefunden! FPS-Script wird entfernt.");
			Destroy(this.gameObject);
			return;
		}

		if(sounds_active){

			if(!audio_source){
				print("Keine Audio Source im Objekt " + this.name + " gefunden! Objekt wird entfernt.");
				Destroy(this.gameObject);
				return;
			}

			foreach (AudioClip sound_file in step_sounds){
				if(sound_file == null){
					print("Es fehlt mindestens ein Sound in step_sounds im Spieler '" + this.name + "', daher wurden die Sounds deaktiviert.");
					sounds_active = false;
					return;
				}
			}

			if(jump_allowed){
				if( jump_step_sound == null || jump_land_sound == null ){
					print("Es fehlt jump_land_sound oder jump_step_sound im Spieler '" + this.name + "', daher wurden die Sounds deaktiviert.");
					sounds_active = false;
					return;
				}
			}

		}

	}

	void Start(){

		head_up = false;
		jump_trigger = false;
		use_trigger = false;
		jump_has_ended = true;
		head_2_target_lerp_val=0F;
		head_2_target_lerp_min=0.15F;
		head_2_target_lerp_max=0.65F;

		Cursor.lockState = CursorLockMode.Locked; //Mauszeiger im Fenster einsperren
		Cursor.visible = false; //Mauszeiger unsichtbar
		if(interaction_allowed)crosshair_ui.sprite = crosshair_standard;

		my_rigid_body.linearDamping = 1.0F;
		my_rigid_body.angularDamping = 1.0F;

		cam_standard_pos = cam.transform.localPosition;

		cam.transform.localRotation = Quaternion.Euler( 0F , 0F , 0F );
		transform.rotation = Quaternion.Euler( 0F , 0F , 0F );

	}

	void Update(){

		if(turn_allowed){ //Wenn Drehung / Kopfbewegung erlaubt ist
			
			cam_x_rot_val += ( Input.GetAxis("Mouse Y") * cam_turn_speed ) * -1;
			cam_x_rot_val = Mathf.Clamp( cam_x_rot_val , -cam_x_rot_limit , cam_x_rot_limit ); //Winkel innerhalb des Limits ausrechnen
			cam.transform.localRotation = Quaternion.Euler( cam_x_rot_val , 0F , 0F ); //Blick nach oben und unten ändern

			this.transform.rotation *= Quaternion.Euler( 0F , Input.GetAxis("Mouse X") * cam_turn_speed , 0F ); //seitliche Drehung des Spielers

		}else{ //Wenn Drehung / Kopfbewegung nicht erlaubt ist
			
			cam.transform.localRotation = Quaternion.Slerp( cam.transform.localRotation , Quaternion.Euler( 0F , 0F , 0F ) , cam_turn_speed * Time.deltaTime );
			transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler( 0F , 0F , 0F ) , cam_turn_speed * Time.deltaTime );

		}

		//##START Steuerungsvariablen auslesen######################################
		key_left = Input.GetKey("a");
		key_forward = Input.GetKey("w");
		key_backwards = Input.GetKey("s");
		key_right = Input.GetKey("d");

		key_crouch = Input.GetKey(KeyCode.LeftControl);
		key_jump = Input.GetKeyDown(KeyCode.Space);
		key_run = Input.GetKey(KeyCode.LeftShift);

		if( Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift) )lerp_reset=true;

		key_use = Input.GetKeyDown(use_key);

		if(key_jump)jump_trigger=true;
		if(key_use)use_trigger=true;
		//##ENDE Steuerungsvariablen auslesen######################################

	}

	void FixedUpdate(){

		//##START Variablen Reset######################################
		Vector3 mover_forward = new Vector3(0, 0, 0);
		Vector3 mover_sideways = new Vector3(0, 0, 0);
		Vector3 crouch_scale = new Vector3(1.0F, 1.0F, 1.0F);

		float walk_speed_cur = walk_speed * Time.deltaTime;

		bool jump_action = false;
		bool walking = false;
		bool running = false;
		bool crouching = false;
		//##ENDE Variablen Reset######################################

		//##START Informationen sammeln######################################
		if( jump_allowed && Physics.Raycast( this.transform.position , (transform.up*-1) , out down_ray , down_length ) ){
			floor_touch = true;
			floor_ob = down_ray.transform.gameObject;
		}else if(jump_allowed){
			floor_touch = false;
			floor_ob = null;
		}else{
			floor_touch = true;
		}

		if (Physics.Raycast( cam.transform.position , cam.transform.forward , out hand_ray , hand_range ) ){
			hand_touch = true;
			hand_ob = hand_ray.transform.gameObject;
			// float dist = Vector3.Distance( this.transform.position , hand_ray.point );
		}else{
			hand_touch = false;
			hand_ob = null;
		}
		//##ENDE Informationen sammeln######################################

		//##START Steuerungsinformationen festlegen######################################
		string use_trigger_val = "";

		if(interaction_allowed){

			if( hand_touch ){
				use_trigger_val = use_action_and_ui_texter_set(hand_ray.transform.tag);
			}else{
				use_trigger_val = use_action_and_ui_texter_set("none");
			}

			if(use_trigger){

				use_trigger = false;

				if(use_trigger_val!="none"){

					object[] message_ob_new = new object[4];
					message_ob_new[0] = this.gameObject.name;
					message_ob_new[1] = use_trigger_val;

					hand_ob.transform.SendMessage("message_system", message_ob_new);

				}

			}

		}

		if(key_right){
			mover_sideways = this.transform.right * walk_speed_cur;
			walking = true;
		}else if(key_left){
			mover_sideways = -transform.right * walk_speed_cur;
			walking = true;
		}

		if(key_forward){
			mover_forward = this.transform.forward * walk_speed_cur;
			walking = true;
		}else if(key_backwards){
			mover_forward = -transform.forward * walk_speed_cur;
			walking = true;
		}

		Vector3 mover = (mover_forward+mover_sideways);

		if(key_forward && key_right || key_forward && key_left || key_backwards && key_right || key_backwards && key_left){ //Bei diagonaler Fortbewegung Geschwindigkeit drosseln
			mover *= 0.6F;
		}

		if(key_crouch && crouch_allowed){
			crouching=true;
			// crouch_scale = new Vector3( 1.0F , 1.0F - crouch_pos_factor , 1.0F );
			crouch_scale = Vector3.Lerp( this.transform.localScale , new Vector3( original_size_v3.x , original_size_v3.y - crouch_pos_factor , original_size_v3.z ) , Time.deltaTime * crouch_pos_speed );
		}else{
			crouching=false;
			// crouch_scale = new Vector3( 1.0F , 1.0F , 1.0F );
			crouch_scale = Vector3.Lerp( this.transform.localScale , original_size_v3 , Time.deltaTime * crouch_pos_speed );
		}
		
		if(jump_allowed && jump_trigger && jump_has_ended && floor_touch){
			jump_trigger = false;
			jump_has_ended = false;
			jump_timer=0F;
			jump_action = true;
			play_sound(jump_step_sound,1.0F,true);
		}

		if( !jump_has_ended && jump_timer<jump_delay ){
			jump_timer+=Time.deltaTime;
		}else if( !jump_has_ended && jump_timer>=jump_delay && floor_touch ){
			jump_has_ended = true;
		}
		
		if( !floor_touch_before && floor_touch ){
			jump_has_ended = true;
			play_sound(jump_land_sound,1.0F,true);
		}

		floor_touch_before = floor_touch;

		if(walking && crouching){
			walking = true;
			running = false;
			mover *= crouch_speed_factor;
		}else if(walking && key_run){
			walking = false;
			running = true;
			mover *= run_speed_factor;
		}
		//##ENDE Steuerungsinformationen festlegen######################################

		//##START Steuerungsbefehle ausführen######################################
		if(this.transform.localScale != crouch_scale)this.transform.localScale = crouch_scale;
		if( jump_allowed && jump_action )my_rigid_body.AddForce (0, jump_force, 0, ForceMode.Impulse );
		if( move_allowed && (walking || running || crouching) )my_rigid_body.MovePosition( this.transform.position + new Vector3( mover.x , 0 , mover.z ) );
		//##ENDE Steuerungsbefehle ausführen######################################

		//##Start Kopfbewegung ausführen######################################
		if( move_allowed && floor_touch && (walking || running) ){

			if(lerp_reset){
				head_2_target_lerp_val = head_2_target_lerp_min;
				lerp_reset=false;
			}

			float input_val;

			if(running){
				input_val = Time.timeSinceLevelLoad / (head_hop_time / run_speed_factor);
			}else{
				input_val = Time.timeSinceLevelLoad / head_hop_time;
			}

			float distance = head_hop_height * Mathf.Sin(input_val);
			Vector3 cam_target_pos = cam_standard_pos + Vector3.up * distance;
			// print("head_2_target_lerp_val:"+head_2_target_lerp_val);
			if( head_2_target_lerp_val < head_2_target_lerp_max )head_2_target_lerp_val+=Time.deltaTime;
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cam_target_pos, head_2_target_lerp_val);

			if( !head_up && distance > 0 ){
				head_up = true;
			}else if( head_up && distance < 0 ){
				head_up = false;
				if(sounds_active){
					int zufall_sound = (int)Random.Range(0, step_sounds.Count);
					play_sound(step_sounds[zufall_sound],1.0F,true);
				}
			}

		}else{
			head_2_target_lerp_val = head_2_target_lerp_min;
			// print("head_2_target_lerp_val:"+head_2_target_lerp_val);
			cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cam_standard_pos, head_2_target_lerp_val);
		}
		//##ENDE Kopfbewegung ausführen######################################


	}

	string use_action_and_ui_texter_set(string object_tag){

		string text_4_ui = "";
		string result_val = "";

		if( object_tag == "collectable" ){

			text_4_ui = "DRÜCKE " + use_key.ToUpper() + " ZUM EINSAMMELN";
			result_val = "collect";
			crosshair_ui.sprite = crosshair_positive;

		}else if( object_tag == "activatable" ){

			text_4_ui = "DRÜCKE " + use_key.ToUpper() + " ZUM AKTIVIEREN";
			result_val = "activate";
			crosshair_ui.sprite = crosshair_positive;

		}else if( object_tag != "none"){

			text_4_ui = "";
			result_val = "none";
			crosshair_ui.sprite = crosshair_negative;

		}else if( object_tag == "none"){

			text_4_ui = "";
			result_val = "none";
			crosshair_ui.sprite = crosshair_standard;

		}

		if(hand_ui_texter){
			hand_ui_texter.text = text_4_ui;
		}

		return result_val;

	}

	void play_sound(AudioClip sound_file, float vol, bool random_pitch){

		if(sounds_active){

			audio_source.clip = sound_file;
			
			if(random_pitch){
				audio_source.pitch=Random.Range(0.7F, 1.0F);
			}else{
				audio_source.pitch=1.0F;
			}

			audio_source.volume = vol;
			audio_source.loop = false;
			audio_source.PlayOneShot(sound_file,vol);

		}

	}

	void OnDrawGizmos(){

		if(floor_touch){
			Gizmos.color = Color.green;
		}else{
			Gizmos.color = Color.white;
		}

		Gizmos.DrawRay( this.transform.position , ((transform.up*-1) * down_length) );

		if(hand_touch){
			Gizmos.color = Color.red;
		}else{
			Gizmos.color = Color.white;
		}

		if(cam)Gizmos.DrawRay( cam.transform.position , (cam.transform.forward * hand_range) );

	}

}

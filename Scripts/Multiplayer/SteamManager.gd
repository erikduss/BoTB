extends Node

func _ready():
	#we only want to do anything if we are on a compatible playform
	if OS.get_name() == "Android" || OS.get_name() == "iOS":
		print("We are on android or ios")
		queue_free()
		return
	
	
	Steam.steamInit()
	
	var steamRunning = Steam.isSteamRunning()
	
	if !steamRunning:
		print("Steam is not running")
		return
	else:
		print("Steam is running")
	
	var userID = Steam.getSteamID()
	var steamUserName = Steam.getFriendPersonaName(userID)
	
	print("Your Steam name is " + steamUserName)
	
	pass

extends Label

func _ready():
	setName()
	return

func setName():
	#we only want to do anything if we are on a compatible playform
	if OS.get_name() == "Android" || OS.get_name() == "iOS":
		print("We are on android or ios")
		return
	
	if !GDSync.is_gdsync_owner(self):
		print("We are not the owner of this object")
	else:
		print("We are the owner")
	
	var steamRunning = Steam.isSteamRunning()
	
	if !steamRunning:
		return
	
	var userID = Steam.getSteamID()
	var steamUserName = Steam.getFriendPersonaName(userID)
	
	text = steamUserName
	
	pass

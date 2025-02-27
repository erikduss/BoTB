# MIT License

# Copyright (c) 2023-present Poing Studios

# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.

#https://poingstudios.github.io/godot-admob-plugin/ad_formats/rewarded/#__tabbed_1_2
#https://youtu.be/WpVGn7ZasKM?si=K5NBIjn_vyQkxMmF
#
#we need to load the ads automatically instead of having it on 2 button presses.
#We still need to implement it for ios.
#We still need to set it to the ID of the game instead of the test one (the correct ID one wasnt working before).
#We need to reward the player for watching.

extends Control

var rewarded_ad : RewardedAd
var on_user_earned_reward_listener := OnUserEarnedRewardListener.new()
var rewarded_ad_load_callback := RewardedAdLoadCallback.new()
var full_screen_content_callback := FullScreenContentCallback.new()

@export var debugText = Label

func _ready() -> void:
	#we only want to do anything if we are on a compatible playform
	#if OS.get_name() != "Android" && OS.get_name() != "iOS":
	#	print("We are not on android or ios")
	#	queue_free()
	#	return
	
	on_user_earned_reward_listener.on_user_earned_reward = on_user_earned_reward
	
	rewarded_ad_load_callback.on_ad_failed_to_load = on_rewarded_ad_failed_to_load
	rewarded_ad_load_callback.on_ad_loaded = on_rewarded_ad_loaded

	full_screen_content_callback.on_ad_clicked = func() -> void:
		print("on_ad_clicked")
	full_screen_content_callback.on_ad_dismissed_full_screen_content = func() -> void:
		print("on_ad_dismissed_full_screen_content")
		destroy()
		
	full_screen_content_callback.on_ad_failed_to_show_full_screen_content = func(ad_error : AdError) -> void:
		print("on_ad_failed_to_show_full_screen_content")
	full_screen_content_callback.on_ad_impression = func() -> void:
		print("on_ad_impression")
	full_screen_content_callback.on_ad_showed_full_screen_content = func() -> void:
		print("on_ad_showed_full_screen_content")
	
	#The initializate needs to be done only once, ideally at app launch.
	MobileAds.initialize()

func _on_load_pressed():
	addToDebugLabel("Started Loading Ad")
	
	var unit_id : String
	if OS.get_name() == "Android":
		unit_id = "ca-app-pub-3940256099942544/5224354917" #ca-app-pub-5689774419460019~8307809391"
	elif OS.get_name() == "iOS":
		unit_id = "ca-app-pub-3940256099942544/1712485313" #"ca-app-pub-5689774419460019~8986735745"
	
	addToDebugLabel("Loading for: " + unit_id)
	
	RewardedAdLoader.new().load(unit_id, AdRequest.new(), rewarded_ad_load_callback)

func on_rewarded_ad_failed_to_load(adError : LoadAdError) -> void:
	addToDebugLabel("Failed to load Ad: " + adError.message)
	print(adError.message)
	
func on_rewarded_ad_loaded(rewarded_ad : RewardedAd) -> void:
	addToDebugLabel("Loaded Ad successfully")
	print("rewarded ad loaded" + str(rewarded_ad._uid))
	
	addToDebugLabel("The ad ID is: " + str(rewarded_ad._uid))
	rewarded_ad.full_screen_content_callback = full_screen_content_callback

	#var server_side_verification_options := ServerSideVerificationOptions.new()
	#server_side_verification_options.custom_data = "TEST PURPOSE"
	#server_side_verification_options.user_id = "user_id_test"
	#rewarded_ad.set_server_side_verification_options(server_side_verification_options)

	addToDebugLabel("Set the ad")
	self.rewarded_ad = rewarded_ad

func _on_show_pressed():
	addToDebugLabel("Showing ad button pressed")
	if rewarded_ad:
		addToDebugLabel("Showing ad to user")
		rewarded_ad.show(on_user_earned_reward_listener)
	else:
		addToDebugLabel("We dont have an ad to show to user")

func on_user_earned_reward(rewarded_item : RewardedItem):
	addToDebugLabel("User earned reward")
	print("on_user_earned_reward, rewarded_item: rewarded", rewarded_item.amount, rewarded_item.type)

func _on_destroy_pressed():
	destroy()

func destroy():
	addToDebugLabel("Destroying Ad")
	if rewarded_ad:
		rewarded_ad.destroy()
		rewarded_ad = null #need to load again

func addToDebugLabel(textToAdd : String):
	debugText.text = debugText.text + "\n" + textToAdd

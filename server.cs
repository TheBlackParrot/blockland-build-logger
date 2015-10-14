$BuildLogger::SaveDir = "config/server/BuildLogs/";

exec("./logger.cs");

function getSafeDateTime() {
	%full = getDateTime();
	%date = strReplace(getWord(%full, 0), "/", "-");
	%time = strReplace(getWord(%full, 1), ":", ".");

	return %date @ "_" @ %time;
}

package buildLogger {
	// brick logging //
	function fxDTSBrick::onPlant(%this) {
		%this.logBrick("plant");
		return parent::onPlant(%this);
	}

	function fxDTSBrick::onToolBreak(%this, %who) {
		%this.logBrick("remove", %who);
		return parent::onToolBreak(%this, %who);
	}

	// wrench logging //
	function fxDTSBrick::sendWrenchData(%this, %client) {
		%client.lastWrenchedBrick = %this;
		return parent::sendWrenchData(%this, %client);
	}
	function serverCmdSetWrenchData(%client, %data) {
		%client.lastWrenchedBrick.logBrick("properties", %client, %data);
		return parent::serverCmdSetWrenchData(%client, %data);
	}

	// event logging //
	//                         10270    1       0       0        0   ""  11       346
	function serverCmdAddEvent(%client, %delay, %input, %ms, %a, %b, %output, %par1, %par2, %par3, %par4) {
		%client.lastWrenchedBrick.logBrick("events", %client, %input, %a, %output, "DEL" SPC %ms @ "ms" TAB "PARAMS" SPC %par1 SPC %par2 SPC %par3 SPC %par4);
		return parent::serverCmdAddEvent(%client, %delay, %input, %ms, %a, %b, %output, %par1, %par2, %par3, %par4);
	}

	// admin wand logging //
	function AdminWandImage::onHitObject(%this, %who, %arg1, %obj, %pos, %normal) {
		if(%obj.getClassName() $= "fxDTSBrick") {
			%obj.logBrick("remove", %who.client, "AWND");
		}
		return parent::onHitObject(%this, %who, %arg1, %obj, %pos, %normal);
	}

	function GameConnection::autoAdminCheck(%this) {
		%this.buildLogFilename = %this.bl_id @ "/" @ getSafeDateTime();
		return parent::autoAdminCheck(%this);
	}
};
activatePackage(buildLogger);
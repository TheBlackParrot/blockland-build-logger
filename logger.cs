if(!isObject(BuildLoggerFileObj)) {
	new FileObject(BuildLoggerFileObj);
}

function fxDTSBrick::logBrick(%this, %type, %arg1, %arg2, %arg3, %arg4, %arg5) {
	%client = %this.client;

	%filename = $BuildLogger::SaveDir @ %client.buildLogFileName;
	%file = BuildLoggerFileObj;

	if(!isFile(%filename @ "_bricks.log")) {
		%file.openForWrite(%filename @ "_bricks.log");
		%file.writeLine("ACTION" TAB "ID" TAB "DATABLOCK" TAB "POS" TAB "COLOR" TAB "BRICK NAME" TAB "WHO");
		%file.close();
	}
	if(!isFile(%filename @ "_properties.log")) {
		%file.openForWrite(%filename @ "_properties.log");
		%file.writeLine("ID" TAB "DATABLOCK" TAB "POS" TAB "COLOR" TAB "BRICK NAME" TAB "WHO");
		%file.close();
	}
	if(!isFile(%filename @ "_events.log")) {
		%file.openForWrite(%filename @ "_events.log");
		%file.writeLine("ID" TAB "DATABLOCK" TAB "POS" TAB "COLOR" TAB "BRICK NAME" TAB "WHO");
		%file.close();
	}

	switch$(%type) {
		case "plant":
			%file.openForAppend(%filename @ "_bricks.log");
			%file.writeLine("PLNT" TAB %this TAB %this.getDatablock().getName() TAB %this.getPosition() TAB %this.colorID);
			%file.close();

		case "remove":
			%file.openForAppend(%filename @ "_bricks.log");
			%type = "REMV";
			if(%arg2 !$= "") {
				%type = %arg2;
			}
			%file.writeLine(%type TAB %this TAB %this.getDatablock().getName() TAB %this.getPosition() TAB %this.colorID TAB %this.getName() TAB %arg1.bl_id);
			%file.close();

		case "properties":
			%file.openForAppend(%filename @ "_properties.log");
			%file.writeLine(%this TAB %this.getDatablock().getName() TAB %this.getPosition() TAB %this.colorID TAB %this.getName() TAB %arg1.bl_id);

			%light_field = "LDB" SPC getWord(getField(%arg2, 1), 1).uiName;
			%emitter_field = "EDB" SPC getWord(getField(%arg2, 2), 1).uiName;
			%item_field = "IDB" SPC getWord(getField(%arg2, 4), 1).uiName;
			%arg2 = getField(%arg2, 0) TAB %light_field TAB %emitter_field TAB getField(%arg2, 3) TAB %item_field TAB getFields(%arg2, 5);

			%file.writeLine("==" TAB %arg2);
			%file.close();

		case "events":
			%input = %arg2;
			%a = %arg3;
			%output = %arg4;
			%data = %arg5;

			%class = getWord(getField($InputEvent_TargetListfxDTSBrick_[%input],%a),1);
			%name = $OutputEvent_Name[%class, %output];

			%file.openForAppend(%filename @ "_events.log");
			if(getDateTime() !$= %this.lastEventUpdate) {
				%this.lastEventLine = 1;
				%file.writeLine(%this TAB %this.getDatablock().getName() TAB %this.getPosition() TAB %this.colorID TAB %this.getName() TAB %arg1.bl_id);
			}
			%this.lastEventUpdate = getDateTime();

			%file.writeLine("E" @ %this.lastEventLine TAB %class @ "::" @ %name TAB %arg5);
			%this.lastEventLine++;

			%file.close();
	}
}
game "gta5"
fx_version "cerulean"

mono_rt2 'Prerelease expiring 2023-06-30. See https://aka.cfx.re/mono-rt2-preview for info.'

author "Local9 <https://github.com/Local9>"
description "FiveM C# Project"
version "1.0.0"

files {
  "client/Newtonsoft.Json.dll",
  "client/FxEvents.Client.dll",
}

client_scripts {
  "client/*.net.dll",
}

server_scripts {
  "server/*.net.dll",
}

fxevents_debug_mode "yes"

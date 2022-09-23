Localization is done with .hjson (Hjson) files (as per tModLoader) and are all autoloaded.
.hjson files need to match their associated culture name (i.e. en-US.hjson for English (United States)).
We may have as many .hjson files as we'd like, allowing us to split up localization into multiple files across different directories.

Directories:
  ./Config/ - Config headers, field names, and tooltips.
  ./Items/ - Item-related translations (item display names, tooltips, etc.).
  ./Misc/ - Miscellaneous translations that don't fit anywhere else.
  ./NPCs/ - NPC-related translations (NPC display names, bestiary entries, town NPC dialog, etc.).
  ./Vanilla/ - Translations added under the Vanilla keyspace.
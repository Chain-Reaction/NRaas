NRaas_Packer

[b]WHAT IS THIS ?[/b]

It is a cut-down package editor based on the S3PI library.

I found that S3PE was overkill for what the normal day-to-day maintained I needed for my mods, and decided to write my own editor.

It provides a simplified method of importing and exporting my format of STBL files.

CREDITS to the Jones' for the creation of S3PI, on which this application is based.  Google : Simlogical

[b]INSTRUCTIONS[/b]

The "Package" file type can be associated with this application and opened directly via double click in the Windows file explorer.

The application allows for importing files simply by double clicking on the appropriate line on the listing.  You will be prompted to locate a file with a specific name.  If you chose to import one that matches that name, the application will persist its location, so the next time you import it won't need to prompt you.

The naming conventions for the STBL files are the ones I use for my own files.  Names for XML and S3SA are based on the Name provided to the resource.

In all of my recent mods there is a "UnhashedKeys"

All interactions are located in the RIGHT-CLICK popup menu :

	Details
		Prompts to enter the name of the resource.  The Instance Id will automatically be FNV64 hashed to whatever you enter.
		If necessary you can override the Instance Id and enter your own, however any subsequent change to the Name will reset the value.

	Edit
		Available for XML based resources.  The screen will be split and the lower portion will display a text editor containing the XML contents.

	Export
		Prompts you to save the contents of the resource to a file.
		The name will be automatically created based on the Name in the package.

		STBL are exported in <KEY><STR> UNICODE text format.
		The "UnhashedKeys" STBL resource in my mods contains the pre-FNV64 <KEY> values, allowing the export of my STBL with the user-readable codes.
		If you export a STBL without the "UnhashedKeys", the <KEY> will contain the FNV64 value.

	Import
		Prompts you to load the contents of the resource from a file.  Unlike double-clicking, this method will always prompt you with a File dialog.
		If you decide to change the persisted location of the file, this method provides that ability.

		STBL can be imported in either <KEY><STR> UNICODE format, or via standard STBL format.
		The <KEY> value for <KEY><STR> format files can be unhashed key or the FNV64 value in hexadecimal format.
		All text files MUST be in UNICODE.  Any attempt import other formats may produce undefined results.

	S3SA - Change Version
		Prompts to change the version of any "Version 2" S3SA resources in the package.  If no Version 2 resources are present, the option is unavailable.
		"Version 2" is used almost exclusively on Core resources.

	STBL - Import All
		Imports all the STBL resources in the package in one click.  This method is only applicable if you have persisted the location of your STBL file using my naming conventions.
		The English STBL will be chosen for any language that does have it's own file.
		SpanishMexican and SpanishStandard will be used interchangably if one of the files is absent
		PortugueseBrazilian and PortugueseStandard will be used interchangably if one of the files is absent

	STBL - Rename All
		Prompts to rehash the Instances of all the STBL resources to match the FNV64 of a specified text value.
		By default, the Name of the first S3SA resource in the package is used as the hash, as it presumed to be unique amongst all mods.
		If any of the STBL resources are missing, they will be added to the package automatically		

	IMAG - Add New File
		Prompts to import a new IMAG PNG file.  The Name and Instance will be automatically created from the name of the file that is imported.

	XML / LAYO / ITUN - Add New File
		Prompts to import a new XML text file.  The Name and Instance will be automatically created from the name of the file that is imported.

	Delete
		Removes the selected resource from the package.

Package Comparisons

      The "Compare" button at the top of the application can be used to compare the _XML and ITUN contents of two package files.

      When used, the user will be prompted to enter the Left and Right files to compare, and specify where to dump the differences.

      If an _XML or ITUN resource does not match, the LEFT resource is dumped to the results file.  
      If one or the other file is missing a resource, the existing resource will be dumped to the results file.

[b]WARNINGS[/b]

This application is only meant to be used on small packages, such as my own.  You will find that opening the Sims3GameplayData.package will be abysmally slow.  Use S3PE for that sort of work. 

You must have .NET Framework Version 3.5 installed to run this application.

I do not provide Mac support.  If it works under MONO, yippee.  If not, then sorry, you are out of luck.

Have Fun. :D
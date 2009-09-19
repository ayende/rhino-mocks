properties { 
  $base_dir  = resolve-path .
  $lib_dir = "$base_dir\SharedLibs"
  $build_dir = "$base_dir\build" 
  $buildartifacts_dir = "$build_dir\" 
  $sln_file = "$base_dir\Rhino.Mocks.sln" 
  $version = "3.6.0.0"
  $humanReadableversion = "3.6"
  $tools_dir = "$base_dir\Tools"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "Rhino-Mocks"
  $uploadScript = "C:\Builds\Upload\PublishBuild.build"
} 

include .\psake_ext.ps1
	
task default -depends Release

task Clean { 
  remove-item -force -recurse $buildartifacts_dir -ErrorAction SilentlyContinue 
  remove-item -force -recurse $release_dir -ErrorAction SilentlyContinue 
} 

task Init -depends Clean { 
	
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Mocks\Properties\AssemblyInfo.cs" `
		-title "Rhino Mocks $version" `
		-description "Mocking Framework for .NET" `
		-company "Hibernating Rhinos" `
		-product "Rhino Mocks $version" `
		-version $version `
		-copyright "Hibernating Rhinos & Ayende Rahien 2004 - 2009"
		
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Mocks.Tests\Properties\AssemblyInfo.cs" `
		-title "Rhino Mocks Tests $version" `
		-description "Mocking Framework for .NET" `
		-company "Hibernating Rhinos" `
		-product "Rhino Mocks Tests $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2004 - 2009"
		
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Mocks.Tests.Model\Properties\AssemblyInfo.cs" `
		-title "Rhino Mocks Tests Model $version" `
		-description "Mocking Framework for .NET" `
		-company "Hibernating Rhinos" `
		-product "Rhino Mocks Tests Model $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2004 - 2009"
		
	new-item $release_dir -itemType directory 
	new-item $buildartifacts_dir -itemType directory 
	cp $tools_dir\MbUnit\*.* $build_dir
} 

task Compile -depends Init { 
  exec msbuild "/p:OutDir=""$buildartifacts_dir "" $sln_file"
} 

task Test -depends Compile {
  $old = pwd
  cd $build_dir
  &.\MbUnit.Cons.exe "$build_dir\Rhino.Mocks.Tests.dll" /report-type:Html
  if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute tests"
  }
  cd $old		
}

task Merge {
	$old = pwd
	cd $build_dir
	
	Remove-Item Rhino.Mocks.Partial.dll -ErrorAction SilentlyContinue 
	Rename-Item $build_dir\Rhino.Mocks.dll Rhino.Mocks.Partial.dll
	
	& $tools_dir\ILMerge.exe Rhino.Mocks.Partial.dll `
		Castle.DynamicProxy2.dll `
		Castle.Core.dll `
		/out:Rhino.Mocks.dll `
		/t:library `
		"/keyfile:$base_dir\ayende-open-source.snk" `
		"/internalize:$base_dir\ilmerge.exclude"
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to merge assemblies!"
    }
	cd $old
}

task Release -depends Test, Merge {
	& $tools_dir\zip.exe -9 -A -j `
		$release_dir\Rhino.Mocks-$humanReadableversion-Build-$env:ccnetnumericlabel.zip `
		$build_dir\Rhino.Mocks.dll `
		$build_dir\Rhino.Mocks.xml `
		license.txt `
		acknowledgements.txt
	if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute ZIP command"
    }
}

task Upload -depend Release {
	if (Test-Path $uploadScript ) {
		$log = git log -n 1 --oneline		
		msbuild $uploadScript /p:Category=$uploadCategory "/p:Comment=$log" "/p:File=$release_dir\Rhino.Mocks-$humanReadableversion-Build-$env:ccnetnumericlabel.zip"
		
		if ($lastExitCode -ne 0) {
			throw "Error: Failed to publish build"
		}
	}
	else {
		Write-Host "could not find upload script $uploadScript, skipping upload"
	}
}
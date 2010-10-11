properties { 
  $base_dir  = resolve-path .
  $lib_dir = "$base_dir\SharedLibs"
  $build_dir = "$base_dir\build" 
  
  $sln_file = "$base_dir\Rhino.Mocks.sln" 
  $version = "3.6.0.0"
  $humanReadableversion = "3.6"
  $tools_dir = "$base_dir\Tools"
  $release_dir = "$base_dir\Release"
  $uploadCategory = "Rhino-Mocks"
  $uploader = "..\Uploader\S3Uploader.exe"
} 

include .\psake_ext.ps1
	
task default -depends Release

task Clean { 
  remove-item -force -recurse $build_dir -ErrorAction SilentlyContinue 
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
		-copyright "Hibernating Rhinos & Ayende Rahien 2004 - 2009" `
		-internalsVisibleTo "Rhino.Mocks.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001009d1cf4b75b7218b141ac64c15450141b1e5f41f6a302ac717ab9761fa6ae2c3ee0c354c22d0a60ac59de41fa285d572e7cf33c320aa7ff877e2b7da1792fcc6aa4eb0b4d8294a2f74cb14d03fb9b091f751d6dc49e626d74601692c99eab7718ed76a40c36d39af842be378b677e6e4eae973f643d7065241ad86ecc156d81ab"
		
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
	
	Generate-Assembly-Info `
		-file "$base_dir\Rhino.Mocks.GettingStarted\Properties\AssemblyInfo.cs" `
		-title "Rhino Mocks Tests $version" `
		-description "Mocking Framework for .NET" `
		-company "Hibernating Rhinos" `
		-product "Rhino Mocks Tests $version" `
		-version $version `
		-clsCompliant "false" `
		-copyright "Hibernating Rhinos & Ayende Rahien 2004 - 2009"
		
	new-item $release_dir -itemType directory 
	new-item $build_dir -itemType directory 
	cp $tools_dir\xUnit\*.* $build_dir
} 

task Compile -depends Init { 
  & msbuild "$sln_file" "/p:OutDir=$build_dir\\" /p:Configuration=Release
  if ($lastExitCode -ne 0) {
        throw "Error: Failed to execute msbuild"
  }
} 

task Test -depends Compile {
  $old = pwd
  cd $build_dir
  &.\Xunit.console.exe "$build_dir\Rhino.Mocks.Tests.dll"
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


task Upload -depends Release {
	Write-Host "Starting upload"
	if (Test-Path $uploader) {
		$log = $env:push_msg 
    if($log -eq $null -or $log.Length -eq 0) {
      $log = git log -n 1 --oneline		
    }
		&$uploader "$uploadCategory" "$release_dir\Rhino.Mocks-$humanReadableversion-Build-$env:ccnetnumericlabel.zip" "$log"
		
		if ($lastExitCode -ne 0) {
      write-host "Failed to upload to S3: $lastExitCode"
			throw "Error: Failed to publish build"
		}
	}
	else {
		Write-Host "could not find upload script $uploadScript, skipping upload"
	}
}

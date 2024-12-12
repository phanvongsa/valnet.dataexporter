#! /bin/bash
if [ "$#" -lt 2 ]; then
    echo "Usage: $0 <action build|upload|deploy|release> <profile local|dev>"
    exit 1
fi

. ./build.conf

action=$1
profile=$2

#"/mnt/source/valnet.dataexporter/valnet.dataexporter/bin/Release/net8.0/"
### BUILD
#echo $release_file
if [[ "$action" =~ "build" || "$action" == "release" ]]; then
    if [[ -d "$target_release_folder" ]]; then
        echo "Cleaning up old release folder"
        rm -r "$target_release_folder"
    fi

    # > Build
    echo "Building Project"
    dotnet build --configuration Release
        
    # > Copy to Release folder
    
  if [[ "$(find "$source_release_folder" -mindepth 1 -print -quit)" ]]; then
    echo "Build Success"      
    cp -r $source_release_folder $target_release_folder    
    if [[ "$(find "$target_release_folder" -mindepth 1 -print -quit)" ]]; then
      echo "Copy to release folder success"
    else
      echo "Copy to release folder error"
      exit 1
    fi

  else
    echo "Build Error"
    exit 1
  fi
fi

if [[ "$action" =~ "deploy" || "$action" == "release" ]]; then
    echo "Running $app_name with $profile"
    cd $target_release_folder        
    export ASPNETCORE_ENVIRONMENT=$profile
    dotnet ./$release_file
fi

# dotnet build --configuration Release;
# cp ./valnet.dataexporter/bin/Release/net8.0/* /mnt/release/valnet.dataexporter/;
# /mnt/release/valnet.dataexporter/valnet.dataexporter

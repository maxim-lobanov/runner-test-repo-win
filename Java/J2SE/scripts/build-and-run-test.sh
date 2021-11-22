#!/usr/bin/env bash

ver=$(echo $JAVA_PROJECT | sed s@java@@)
PREV_JAVA_HOME="$JAVA_HOME"
export JAVA_HOME=$(echo ${!JAVA_VAR})
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    sudo update-java-alternatives -s $JAVA_HOME
elif [[ ! "$OSTYPE" == "darwin"* ]]; then
    # The version directory could be either x.y.zzz-n or x.y.zzz-n.m
    versionDir=$(echo "${JAVA_HOME}" | grep -Eo "Java_.*_jdk.[0-9]+.*-[0-9]+(\.[0-9]+){0,}")
    prevVersion=$(echo "${PREV_JAVA_HOME}" | grep -Eo "Java_.*_jdk.[0-9]+.*-[0-9]+(\.[0-9]+){0,}")

    versionDir=$(echo $versionDir | tr '\' '/')
    prevVersion=$(echo $prevVersion | tr '\' '/')

    # Substitute the previous version with the current one in the PATH
    export PATH=$(echo $PATH | sed s@$prevVersion@$versionDir@)
    java -version
    echo "PATH is $PATH"
fi
echo Current JAVA_HOME is: "$JAVA_HOME"

if [ "$ver" -eq "7" ]; then
    if [[ "$OSTYPE" == "darwin"* ]]; then
        sed -i '' "s/gradle-6.4-bin.zip/gradle-4.10.2-bin.zip/g" 'gradle/wrapper/gradle-wrapper.properties'
    else
        sed -i "s/gradle-6.4-bin.zip/gradle-4.10.2-bin.zip/g" gradle/wrapper/gradle-wrapper.properties
    fi
fi

cd $JAVA_PROJECT
../gradlew clean
../gradlew compileJava

if [ "$ver" -ge "12" ]; then
    java --enable-preview -cp build/classes/java/main $JAVA_CLASS | grep "Current Java version is \"$ver\.[0-9]"
else
    java -cp build/classes/java/main $JAVA_CLASS | grep -E "Current Java version is \"(1\.)?$ver\.[0-9]"
fi

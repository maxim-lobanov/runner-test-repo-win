#!/usr/bin/env bash

JAVA_HOME=$(echo ${!JAVA_VAR})
cd $JAVA_PROJECT
java -cp build/classes/java/main $JAVA_CLASS
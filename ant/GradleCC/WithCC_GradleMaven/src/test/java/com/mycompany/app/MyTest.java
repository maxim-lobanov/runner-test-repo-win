package com.mycompany.app;

import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;
import junit.framework.Assert;
import java.lang.annotation.Annotation;

public class MyTest
    extends TestCase
{
    public MyTest( String testName )
    {
        super( testName );
    }

    public static Test suite()
    {
        return new TestSuite( MyTest.class );
    }

	public void testApp3()
	{
 		Assert.assertEquals(App.add(2,3), 5);
	}
	
	public void testApp4()
	{
 		Assert.assertEquals(App2.sub(2,3), -1);
	}
	
	public void HelloDude() {
 
		Assert.assertEquals("Hello World 2", "Hello World 2");
 
	} 
}
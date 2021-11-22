package com.mycompany.app;

import junit.framework.Test;
import junit.framework.TestCase;
import junit.framework.TestSuite;
import junit.framework.Assert;
import java.lang.annotation.Annotation;

/**
 * Unit test for simple App.
 */
public class AppTest 
    extends TestCase
{
    /**
     * Create the test case
     *
     * @param testName name of the test case
     */
    public AppTest( String testName )
    {
        super( testName );
    }
	
    /**
     * @return the suite of tests being tested
     */
    public static Test suite()
    {
        return new TestSuite( AppTest.class );
    }
	
    /**
     * Rigourous Test :-)
     */
    public void testApp()
    {
        Assert.assertTrue( true );
    }
	

    /**
     * Rigourous Test :-)
     */
    public void testApp2()
    {
        Assert.assertTrue( true );
    }
}

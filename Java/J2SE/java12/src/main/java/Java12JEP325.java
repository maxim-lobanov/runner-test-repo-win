
public class Java12JEP325 {

	public static void main(String[] args) {
		System.out.println("Current Java version is \"" + System.getProperty("java.version") + "\"");

		// Test JEP 325: Switch Expressions
		final int day = (int)(Math.random()*10);
		final String attr = switch (day) {
			case 1, 3, 5, 7 -> "odd";
			case 2, 4, 6, 8 -> "even";
	  	default -> {
				break "invalid";
			}
		};
		System.out.println(attr);
	}
}

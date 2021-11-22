import java.util.Set;
import java.util.List;

public class Java9FactoryMethods {

    public static void main(String[] args) {
        System.out.println("Current Java version is \"" + System.getProperty("java.version") + "\"");

        // Test
        Set<Integer> ints = Set.of(1, 2, 3);
        System.out.println(ints.toString());

        List<String> strings = List.of("first", "second");
        System.out.println(strings.toString());

        System.out.println("Java 9 test succeeded");
    }
}
import unittest
from selenium import webdriver
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.wait import WebDriverWait

class WebDriverPythonTests(unittest.TestCase):
    def setUp(self):
        print("Launching Chrome browser")
        self.chrome_options = Options()
        self.chrome_options.add_argument('--no-sandbox')
        self.chrome_options.add_argument('--headless')
        self.chrome_options.add_argument('--disable-dev-shm-usage')
        self.driver = webdriver.Chrome(chrome_options = self.chrome_options)
        self.driver.maximize_window()
        self.driver.implicitly_wait(10)

    def test_search_in_python_org(self):
        driver = self.driver
        driver.get("http://www.python.org")
        print("Title of http://www.python.org page: " + driver.title)
        assert "Python" in driver.title
        elem = driver.find_element_by_name("q")
        elem.clear()
        print("Trying to find pycon on python.org")
        elem.send_keys("pycon")
        elem.send_keys(Keys.RETURN)
        print("Current URL: " + driver.current_url)
        assert "No results found." not in driver.page_source

    def test_wiki(self):
        driver = self.driver
        driver.get("https://en.wikipedia.org/wiki/Main_Page")
        wait = WebDriverWait(driver,100)
        searchInput = driver.find_element_by_id("searchInput")
        wait.until(EC.presence_of_element_located((By.ID,"searchInput")))
        searchInput.send_keys("Software")
        searchButton = driver.find_element_by_id("searchButton")
        wait.until(EC.element_to_be_clickable((By.ID,"searchButton")))
        searchButton.submit()
        wait.until(EC.text_to_be_present_in_element((By.TAG_NAME,"body"), "Computer software"))
 
    def tearDown(self):
        print("Closing chrome browser")
        self.driver.close()

if __name__ == "__main__":
    unittest.main()

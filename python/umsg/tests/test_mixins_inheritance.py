#
# NOTES:
#  - umsg stores per-module state by design
#  - make sure you propagate, or caplog won't see messages!
#  - we need to set logging to the lowest, DEBUG, or we can't test debug msgs
#
import logging

from pyassert import *
import pytest
import umsg
from umsg.mixins import LoggingMixin



LNAME = 'Inheritance'


class Parent(LoggingMixin):
    def __init__(self):
        super().__init__(prefix='Parent')


class Child(Parent):
    def __init__(self):
        super().__init__()
        self._umsg_log_prefix = 'Child'


class GrandChild(Child):
    def __init__(self):
        super().__init__()
        self._umsg_log_prefix = 'GrandChild'



class TestInheritance:
    @classmethod
    def setup_class(cls):
        umsg.init(logger_name=LNAME,
                  level=logging.DEBUG,
                  propagate=True
                 )
        cls.parent = Parent()
        cls.child = Child()
        cls.grandchild = GrandChild()

    # ----------------------------------------------------------------- Parent -
    @pytest.mark.dependency(name='parent')
    def test_parent_setup(self):
        class_logger = self.parent._umsg_logger
        class_prefix = self.parent._umsg_log_prefix

        assert_that(class_logger.name).is_equal_to(LNAME)
        assert_that(class_logger.getEffectiveLevel()).is_equal_to(logging.DEBUG)
        assert_that(class_prefix).is_equal_to('Parent')

    @pytest.mark.dependency(depends=['parent'])
    def test_parent_profile(self):
        logger = umsg.init(logger_name=LNAME)

        assert_that(logger).is_identical_to(self.parent._umsg_logger)

    @pytest.mark.dependency(depends=['parent'])
    def test_parent_prefix_setting(self, caplog):
        prefix = self.parent._umsg_log_prefix
        msg = 'who am i?'
        self.parent.log(msg)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

    # ------------------------------------------------------------------ Child -
    @pytest.mark.dependency(name='child', depends=['parent'])
    def test_child_setup(self):
        class_logger = self.child._umsg_logger
        class_prefix = self.child._umsg_log_prefix

        assert_that(class_logger.name).is_equal_to(LNAME)
        assert_that(class_logger.getEffectiveLevel()).is_equal_to(logging.DEBUG)
        assert_that(class_prefix).is_equal_to('Child')

    @pytest.mark.dependency(depends=['child'])
    def test_child_profile(self):
        logger = umsg.init(logger_name=LNAME)

        assert_that(logger).is_identical_to(self.parent._umsg_logger)
        assert_that(logger).is_identical_to(self.child._umsg_logger)

    @pytest.mark.dependency(depends=['child'])
    def test_child_prefix_setting(self, caplog):
        prefix = self.child._umsg_log_prefix
        msg = 'who am i?'
        self.child.log(msg)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

    # ------------------------------------------------------------ Grand Child -
    @pytest.mark.dependency(name='grandchild', depends=['parent', 'child'])
    def test_grandchild_setup(self):
        class_logger = self.grandchild._umsg_logger
        class_prefix = self.grandchild._umsg_log_prefix

        assert_that(class_logger.name).is_equal_to(LNAME)
        assert_that(class_logger.getEffectiveLevel()).is_equal_to(logging.DEBUG)
        assert_that(class_prefix).is_equal_to('GrandChild')

    @pytest.mark.dependency(depends=['grandchild'])
    def test_grandchild_profile(self):
        logger = umsg.init(logger_name=LNAME)

        assert_that(logger).is_identical_to(self.parent._umsg_logger)
        assert_that(logger).is_identical_to(self.child._umsg_logger)
        assert_that(logger).is_identical_to(self.grandchild._umsg_logger)

    @pytest.mark.dependency(depends=['grandchild'])
    def test_grandchild_prefix_setting(self, caplog):
        prefix = self.grandchild._umsg_log_prefix
        msg = 'who am i?'
        self.grandchild.log(msg)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

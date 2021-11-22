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



class Base(LoggingMixin):
    def __init__(self):
        super().__init__(prefix='Base')


class TestSingleInstance:
    @classmethod
    def setup_class(cls):
        umsg.init(logger_name=LNAME,
                  level=logging.DEBUG,
                  propagate=True
                 )
        cls.testclass = Base()

    @pytest.mark.dependency(name='setup')
    def test_setup(self):
        class_logger = self.testclass._umsg_logger
        class_prefix = self.testclass._umsg_log_prefix

        assert_that(class_logger.name).is_equal_to(LNAME)
        assert_that(class_logger.getEffectiveLevel()).is_equal_to(logging.DEBUG)
        assert_that(class_prefix).is_equal_to('Base')

    @pytest.mark.dependency(depends=['setup'])
    def test_mixin_profile(self):
        logger = umsg.init(logger_name=LNAME)

        # as long as we're getting back the same instance, everything else is fine
        assert_that(logger).is_identical_to(self.testclass._umsg_logger)

    @pytest.mark.dependency(depends=['setup'])
    def test_mixin_prefix_setting(self, caplog):
        prefix = self.testclass._umsg_log_prefix
        msg = 'mixing mixins'
        self.testclass.log(msg)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

    @pytest.mark.dependency(depends=['setup'])
    def test_mixin_prefix_inline(self, caplog):
        prefix = 'inline'
        msg = 'mixing mixins'
        self.testclass.log(msg, prefix=prefix)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

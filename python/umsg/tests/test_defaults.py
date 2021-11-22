#
# NOTES:
#  - umsg stores per-module state by design
#  - make sure you propagate, or caplog won't see messages!
#  - we need to set logging to the lowest, DEBUG, or we can't test debug msgs
#
import logging
import inspect

from pyassert import *
import pytest
import umsg



class TestDefaults:
    @pytest.mark.dependency(name='setup')
    def test_init_default_name(self):
        logger = umsg.init()
        s = inspect.stack()[0]
        m = inspect.getmodule(s[0])

        assert_that(logger.name).is_equal_to(m.__name__)
        assert_that(umsg.get_attr('logger').name).is_equal_to(m.__name__)

    @pytest.mark.dependency(depends=['setup'])
    def test_init_default_handler(self):
        # logger is already setup for this module
        logger = umsg.get_attr('logger')

        assert_that(logger.handlers[0]).is_instance_of(logging.NullHandler)

    @pytest.mark.dependency(depends=['setup'])
    def test_init_default_level(self):
        logger = umsg.get_attr('logger')

        assert_that(umsg.get_attr('level')).is_equal_to(logging.INFO)

#
# NOTES:
#  - umsg stores per-module state by design
#  - make sure you propagate, or caplog won't see messages!
#  - we need to set logging to the lowest, DEBUG, or we can't test debug msgs
#
import logging
import logging.handlers
import inspect

from pyassert import *
import pytest
import umsg



class TestInit:
    @pytest.mark.dependency(name='setup')
    def test_init(self):
        logger = umsg.init(level='debug')
        s = inspect.stack()[0]
        m = inspect.getmodule(s[0])

        assert_that(logger.name).is_equal_to(m.__name__)
        assert_that(umsg.get_attr('logger').name).is_equal_to(m.__name__)

    @pytest.mark.dependency(depends=['setup'])
    def test_init_handlers(self):
        # logger is already setup for this module
        logger = umsg.get_attr('logger')

        # add instance
        umsg.add_handler(logging.StreamHandler())

        # add class
        umsg.add_handler(logging.handlers.SocketHandler, **{'host': '127.0.0.1', 'port': 99})

        assert_that(logger.handlers[0]).is_instance_of(logging.NullHandler)
        assert_that(logger.handlers[1]).is_instance_of(logging.StreamHandler)
        assert_that(logger.handlers[2]).is_instance_of(logging.handlers.SocketHandler)

    @pytest.mark.dependency(depends=['setup'])
    def test_init_level(self):
        logger = umsg.get_attr('logger')

        assert_that(umsg.get_attr('level')).is_equal_to(logging.DEBUG)

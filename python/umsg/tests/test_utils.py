#
# NOTES:
#
import logging

from pyassert import *
import pytest
import umsg



loglevel_params = [
    pytest.param('CRIT', logging.CRITICAL),
    pytest.param('critical', logging.CRITICAL),
    pytest.param(logging.CRITICAL, logging.CRITICAL),
    pytest.param(50, logging.CRITICAL),

    pytest.param('ERROR', logging.ERROR),
    pytest.param(logging.ERROR, logging.ERROR),
    pytest.param(40, logging.ERROR),

    pytest.param('WARN', logging.WARNING),
    pytest.param('warning', logging.WARNING),
    pytest.param(logging.WARNING, logging.WARNING),
    pytest.param(30, logging.WARNING),

    pytest.param('DEBUG', logging.DEBUG),
    pytest.param('debug', logging.DEBUG),
    pytest.param(logging.DEBUG, logging.DEBUG),
    pytest.param(10, logging.DEBUG),

    pytest.param('INFO', logging.INFO),
    pytest.param('info', logging.INFO),
    pytest.param('nothing', logging.INFO),
    pytest.param('invalid', logging.INFO),
    pytest.param(logging.INFO, logging.INFO),
    pytest.param(20, logging.INFO),
]


class TestDefaults:
    @pytest.mark.parametrize('level,logginglevel', loglevel_params)
    def test_log_level(self, level, logginglevel):
        _level = umsg.util.log_level(level)

        assert_that(_level).is_equal_to(logginglevel)

#
# NOTES:
#  - umsg stores per-module state by design
#  - make sure you propagate, or caplog won't see messages!
#  - we need to set logging to the lowest, DEBUG, or we can't test debug msgs
#
import datetime
import logging
import re

from pyassert import *
import pytest
import umsg



loglevel_params = [
    pytest.param('debug msg', 'debug', logging.DEBUG,
                 marks=pytest.mark.dependency(name='debug', depends=['default'])
                ),
    pytest.param('info msg', 'info', logging.INFO,
                 marks=pytest.mark.dependency(name='info', depends=['debug'])
                ),
    pytest.param('warning msg', 'warning', logging.WARNING,
                 marks=pytest.mark.dependency(name='warn', depends=['info'])
                ),
    pytest.param('error msg', 'error', logging.ERROR,
                 marks=pytest.mark.dependency(name='error', depends=['warn'])
                ),
    pytest.param('critical msg', 'critical', logging.CRITICAL,
                 marks=pytest.mark.dependency(name='crit', depends=['error'])
                )
]



class TestMessaging:
    @classmethod
    def setup_class(cls):
        umsg.set_attr('logger_name', 'TestMessaging')
        umsg.set_attr('level', logging.DEBUG)
        umsg.set_attr('propagate', True)
        logger = umsg.init()

    @pytest.mark.dependency(name='setup')
    def test_setup(self):
        logger = umsg.get_attr('logger')

        assert_that(logger.name).is_equal_to('TestMessaging')
        assert_that(logger.getEffectiveLevel()).is_equal_to(logging.DEBUG)

    @pytest.mark.dependency(name='default', depends=['setup'])
    def test_log_default(self, caplog):
        msg = 'info'
        umsg.log(msg)

        assert_that(caplog.record_tuples[0][1]).is_equal_to(logging.INFO)
        assert_that(caplog.record_tuples[0][2]).is_equal_to(msg)

    @pytest.mark.parametrize('msg,level,logginglevel', loglevel_params)
    def test_log_explicit(self, caplog, msg, level, logginglevel):
        umsg.log(msg, level)

        assert_that(caplog.record_tuples[0][1]).is_equal_to(logginglevel)
        assert_that(caplog.record_tuples[0][2]).is_equal_to(msg)

    @pytest.mark.dependency(depends=['setup'])
    def test_prefix_setting(self, caplog):
        prefix = 'default'
        msg = 'text'
        umsg.set_attr('msg_prefix', prefix)
        umsg.log(msg)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

    @pytest.mark.dependency(depends=['setup'])
    def test_prefix_inline(self, caplog):
        prefix = 'default'
        msg = 'txt'
        umsg.log(msg, prefix=prefix)

        expected = '[{}] {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

    @pytest.mark.dependency(name='default', depends=['setup'])
    def test_prefix_formatting(self, caplog):
        prefix = 'inline'
        msg = 'text'
        umsg.set_attr('msg_prefix_format', '<{prefix}> ')
        umsg.log(msg, prefix=prefix)

        expected = '<{}> {}'.format(prefix, msg)

        assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

    @pytest.mark.dependency(depends=['default'])
    def test_formatter(self, capsys):
        prefix = 'fmt'
        msg = 'stream it'
        umsg.add_handler(logging.StreamHandler)
        umsg.log(msg, prefix=prefix)
        date = datetime.date.today().strftime('%Y-%m-%d')

        expected = '{} ([\\d]{{2}}:?){{3}} - INFO - <{}> {}'.format(date, prefix, msg)
        captured = capsys.readouterr()

        assert(re.match(expected, captured.err))

    @pytest.mark.dependency(depends=['setup'])
    def test_verbose(self, capsys):
        msg = 'print it'
        end = '\n'
        umsg.log(msg, 'verbose')

        expected = '{}{}'.format(msg, end)
        captured = capsys.readouterr()

        assert_that(captured.out).is_equal_to(expected)

    @pytest.mark.dependency(depends=['setup'])
    def test_verbose_end(self, capsys):
        msg = 'print this'
        end = '.'
        umsg.log(msg, 'verbose', end=end)

        expected = '{}{}'.format(msg, end)
        captured = capsys.readouterr()

        assert_that(captured.out).is_equal_to(expected)

    @pytest.mark.dependency(depends=['setup'])
    def test_verbose_prefix_ignored(self, capsys):
        prefix = 'inline'
        msg = 'print this'
        end = '\n'
        umsg.log(msg, 'verbose', prefix=prefix)

        expected = '{}{}'.format(msg, end)
        captured = capsys.readouterr()

        assert_that(captured.out).is_equal_to(expected)

#
# NOTES:
#  - umsg stores per-module state by design
#  - make sure you propagate, or caplog won't see messages!
#  - we need to set logging to the lowest, DEBUG, or we can't test debug msgs
#
# ref: https://fuhm.net/super-harmful/
# ref: https://en.wikipedia.org/wiki/Fragile_base_class
#
import logging

from pyassert import *
import pytest
import umsg
from umsg.mixins import LoggingMixin



LNAME = 'Cooperation'



class NonCooperative:
    def __init__(self, a=1, b=2):
        super().__init__()
        print(a, b)


class Cooperative:
    def __init__(self, *args, a=1, b=2, **kwargs):
        super().__init__(*args, **kwargs)
        print(a, b)



def setup_module():
    umsg.init(logger_name=LNAME, level=logging.DEBUG, propagate=True)


# ------------------------------------------------------------- NonCooperative -
# when not all classes cooperate, order becomes important
def test_noncooperative_proper_order(caplog):
    prefix = 'test-01'
    msg = 'test-01'

    class tclass(LoggingMixin, NonCooperative):
        def __init__(self):
            super().__init__(a=5, prefix=prefix, b=4)
            self.log(msg)

    test = tclass()
    expected = '[{}] {}'.format(prefix, msg)

    assert_that(test._umsg_logger).is_identical_to(umsg.get_attr('logger'))
    assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)


# we expect an error here because NonCooperative doesn't accept downstream
# parameters, and doesn't support good MRO behavior
def test_noncooperative_error():
    prefix = 'test-02'
    msg = 'test-02'

    class tclass(NonCooperative, LoggingMixin):
        def __init__(self):
            super().__init__(a=5, prefix=prefix, b=4)
            self.log(msg)

    with pytest.raises(TypeError) as e:
        test = tclass()

    assert_that(str(e.value)).contains('unexpected keyword argument')


def test_noncooperative_manual_init(caplog):
    prefix = 'test-03'
    msg = 'test-03'

    class tclass(NonCooperative, LoggingMixin):
        def __init__(self):
            super().__init__(a=5, b=4)
            LoggingMixin.__init__(self, prefix=prefix)
            self.log(msg)

    test = tclass()
    expected = '[{}] {}'.format(prefix, msg)

    assert_that(test._umsg_logger).is_identical_to(umsg.get_attr('logger'))
    assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)


# ---------------------------------------------------------------- Cooperative -
# when all classes cooperate, order doesn't matter
def test_cooperative_mixin_first(caplog):
    prefix = 'test-04'
    msg = 'test-04'

    class tclass(LoggingMixin, Cooperative):
        def __init__(self):
            super().__init__(a=5, prefix=prefix, b=4)
            self.log(msg)

    test = tclass()
    expected = '[{}] {}'.format(prefix, msg)

    assert_that(test._umsg_logger).is_identical_to(umsg.get_attr('logger'))
    assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)


def test_cooperative_mixin_last(caplog):
    prefix = 'test-04'
    msg = 'test-04'

    class tclass(Cooperative, LoggingMixin):
        def __init__(self):
            super().__init__(a=5, prefix=prefix, b=4)
            self.log(msg)

    test = tclass()
    expected = '[{}] {}'.format(prefix, msg)

    assert_that(test._umsg_logger).is_identical_to(umsg.get_attr('logger'))
    assert_that(caplog.record_tuples[0][2]).is_equal_to(expected)

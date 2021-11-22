# Copyright © 2020 R.A. Stern
# SPDX-License-Identifier: LGPL-3.0-or-later

import umsg
from .util import _caller



# -------------------------------------------------------------------- Classes -
class LoggingMixin:
    """LoggingMixin provides an interface to the module logging functions.

    Class instance level methods for calling the module level logging functions.
    Two private override attributes are provided for the class.

    Attributes:
        _umsg_logger (:py:class:`logging.Logger`): Logger object override.
        _umsg_log_prefix (str): Log message prefix override
    """
    def __init__(self, *args, prefix=None, logger=None, **kwargs):
        # try to play nice with potentially uncooperative classes
        try:
            super().__init__(*args, **kwargs)
        except TypeError as e:
            try:
                super().__init__()
            except TypeError as e:
                pass

        if logger:
            umsg.set_attr('logger', logger)
        else:
            umsg.init()

        self._umsg_logger = umsg.get_attr('logger')
        self._umsg_log_prefix = prefix or umsg.get_attr('msg_prefix')

    def _msg(self, *args, **kwargs):
        """Inteface for module level :py:func:`umsg._msg` function."""
        if 'prefix' not in kwargs:
            kwargs['prefix'] = self._umsg_log_prefix

        if 'logger' not in kwargs:
            kwargs['logger'] = self._umsg_logger

        umsg.log(*args, **kwargs)

    def log(self, *args, **kwargs):
        """Interface for module level :py:func:`umsg.log` function."""
        self._msg(*args, **kwargs)
